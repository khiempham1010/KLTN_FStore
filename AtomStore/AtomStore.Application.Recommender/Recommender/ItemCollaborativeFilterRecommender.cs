using AtomStore.Application.Recommender.Interfaces;
using AtomStore.Application.Recommender.Objects;
using AtomStore.Application.Recommender.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Recommender.Recommender
{
    public class ItemCollaborativeFilterRecommender : IRecommender
    {
        private IComparer comparer;
        private IRater rater;
        private UserProductRatingsTable ratings;
        private double[][] transposedRatings;

        private int neighborCount;

        public ItemCollaborativeFilterRecommender(IComparer itemComparer, IRater implicitRater, int numberOfNeighbors)
        {
            comparer = itemComparer;
            rater = implicitRater;
            neighborCount = numberOfNeighbors;
        }

        private void FillTransposedRatings()
        {
            int features = ratings.Users.Count;
            transposedRatings = new double[ratings.ProductIndexToID.Count][];

            // Precompute a transposed ratings matrix where each row becomes an product and each column a user or tag
            for (int a = 0; a < ratings.ProductIndexToID.Count; a++)
            {
                transposedRatings[a] = new double[features];

                for (int f = 0; f < features; f++)
                {
                    transposedRatings[a][f] = ratings.Users[f].ProductRatings[a];
                }
            }
        }

        private List<int> GetHighestRatedProductsForUser(int userIndex)
        {
            List<Tuple<int, double>> items = new List<Tuple<int, double>>();

            for (int productIndex = 0; productIndex < ratings.ProductIndexToID.Count; productIndex++)
            {
                // Create a list of every product this user has viewed
                if (ratings.Users[userIndex].ProductRatings[productIndex] != 0)
                {
                    items.Add(new Tuple<int, double>(productIndex, ratings.Users[userIndex].ProductRatings[productIndex]));
                }
            }

            // Sort the products by rating
            items.Sort((c, n) => n.Item2.CompareTo(c.Item2));

            return items.Select(x => x.Item1).ToList();
        }

        public bool Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            ratings = ubt.GetUserProductRatingsTable(rater);
            if (ratings != null)
            {
                List<ProductTagCounts> productTags = ubt.GetProductTagCounts();
                ratings.AppendProductFeatures(productTags);

                FillTransposedRatings();
                return true;
            }
            return false;
        }

        public double GetRating(Guid userId, int productId)
        {
            int userIndex = ratings.UserIndexToID.IndexOf(userId);
            int productIndex = ratings.ProductIndexToID.IndexOf(productId);

            var userRatings = ratings.Users[userIndex].ProductRatings.Where(x => x != 0);
            var productRatings = ratings.Users.Select(x => x.ProductRatings[productIndex]).Where(x => x != 0);

            double averageUser = userRatings.Count() > 0 ? userRatings.Average() : 0;
            double averageProduct = productRatings.Count() > 0 ? productRatings.Average() : 0;

            // For now, just return the average rating across this user and product
            return averageProduct > 0 && averageUser > 0 ? (averageUser + averageProduct) / 2.0 : Math.Max(averageUser, averageProduct);
        }

        public List<Suggestion> GetSuggestions(Guid userId, int numSuggestions)
        {
            int userIndex = ratings.UserIndexToID.IndexOf(userId);
            List<int> products = GetHighestRatedProductsForUser(userIndex).Take(5).ToList();
            List<Suggestion> suggestions = new List<Suggestion>();

            foreach (int productIndex in products)
            {
                int productId = ratings.ProductIndexToID[productIndex];
                List<ProductRating> neighboringProducts = GetNearestNeighbors(productId, neighborCount);

                foreach (ProductRating neighbor in neighboringProducts)
                {
                    int neighborProductIndex = ratings.ProductIndexToID.IndexOf(neighbor.ProductID);

                    double averageProductRating = 0.0;
                    int count = 0;
                    for (int userRatingIndex = 0; userRatingIndex < ratings.UserIndexToID.Count; userRatingIndex++)
                    {
                        if (transposedRatings[neighborProductIndex][userRatingIndex] != 0)
                        {
                            averageProductRating += transposedRatings[neighborProductIndex][userRatingIndex];
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        averageProductRating /= count;
                    }

                    suggestions.Add(new Suggestion(userId, neighbor.ProductID, averageProductRating));
                }
            }

            suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));
            var sugg = suggestions.GroupBy(x => x.ProductID).Select(y => y.FirstOrDefault()).ToList();
            return sugg;
        }

        public List<Suggestion> GetSuggestions(Guid userId, int pId, int numSuggestions)
        {
            int userIndex = ratings.UserIndexToID.IndexOf(userId);
            int productIndex = ratings.ProductIndexToID.IndexOf(pId);
            List<Suggestion> suggestions = new List<Suggestion>();


            int productId = ratings.ProductIndexToID[productIndex];
            List<ProductRating> neighboringProducts = GetNearestNeighbors(productId, neighborCount);

            foreach (ProductRating neighbor in neighboringProducts)
            {
                int neighborProductIndex = ratings.ProductIndexToID.IndexOf(neighbor.ProductID);

                double averageProductRating = 0.0;
                int count = 0;
                for (int userRatingIndex = 0; userRatingIndex < ratings.UserIndexToID.Count; userRatingIndex++)
                {
                    if (transposedRatings[neighborProductIndex][userRatingIndex] != 0)
                    {
                        averageProductRating += transposedRatings[neighborProductIndex][userRatingIndex];
                        count++;
                    }
                }
                if (count > 0)
                {
                    averageProductRating /= count;
                }

                suggestions.Add(new Suggestion(userId, neighbor.ProductID, averageProductRating));
            }


            suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));
            var sugg = suggestions.GroupBy(x => x.ProductID).Select(y => y.FirstOrDefault()).Take(numSuggestions).ToList();
            return sugg;
        }

        private List<ProductRating> GetNearestNeighbors(int productId, int numProducts)
        {
            List<ProductRating> neighbors = new List<ProductRating>();
            int mainProductIndex = ratings.ProductIndexToID.IndexOf(productId);

            for (int productIndex = 0; productIndex < ratings.ProductIndexToID.Count; productIndex++)
            {
                int searchProductId = ratings.ProductIndexToID[productIndex];

                double score = comparer.CompareVectors(transposedRatings[mainProductIndex], transposedRatings[productIndex]);

                neighbors.Add(new ProductRating(searchProductId, score));
            }

            neighbors.Sort((c, n) => n.Rating.CompareTo(c.Rating));

            return neighbors.Take(numProducts).ToList();
        }

        public void Save(string file)
        {
            if (!Directory.Exists($@"\uploaded\recommendation\"))
            {
                Directory.CreateDirectory($@"\uploaded\recommendation");
            }
            using (FileStream fs = File.Create(file))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Compress))
            using (StreamWriter w = new StreamWriter(zip))
            {
                w.WriteLine(ratings.Users.Count);
                w.WriteLine(ratings.Users[0].ProductRatings.Length);

                foreach (UserProductRatings t in ratings.Users)
                {
                    w.WriteLine(t.UserID);

                    foreach (double v in t.ProductRatings)
                    {
                        w.WriteLine(v);
                    }
                }

                w.WriteLine(ratings.UserIndexToID.Count);

                foreach (Guid i in ratings.UserIndexToID)
                {
                    w.WriteLine(i);
                }

                w.WriteLine(ratings.ProductIndexToID.Count);

                foreach (int i in ratings.ProductIndexToID)
                {
                    w.WriteLine(i);
                }
            }
        }

        public void Load(string file)
        {
            ratings = new UserProductRatingsTable();

            using (FileStream fs = File.OpenRead(file))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Decompress))
            using (StreamReader r = new StreamReader(zip))
            {
                long total = long.Parse(r.ReadLine());
                int features = int.Parse(r.ReadLine());

                for (long i = 0; i < total; i++)
                {
                    Guid userId = Guid.Parse(r.ReadLine());
                    UserProductRatings uat = new UserProductRatings(userId, features);

                    for (int x = 0; x < features; x++)
                    {
                        uat.ProductRatings[x] = double.Parse(r.ReadLine());
                    }

                    ratings.Users.Add(uat);
                }

                total = int.Parse(r.ReadLine());

                for (int i = 0; i < total; i++)
                {
                    ratings.UserIndexToID.Add(Guid.Parse(r.ReadLine()));
                }

                total = int.Parse(r.ReadLine());

                for (int i = 0; i < total; i++)
                {
                    ratings.ProductIndexToID.Add(int.Parse(r.ReadLine()));
                }
            }

            FillTransposedRatings();
        }
    }
}
