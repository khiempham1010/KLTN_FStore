using AtomStore.Application.Recommender.Algorithms;
using AtomStore.Application.Recommender.Interfaces;
using AtomStore.Application.Recommender.Objects;
using AtomStore.Application.Recommender.Parsers;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Recommender.Recommender
{
    public class UserCollaborativeFilterRecommender: IRecommender
    {
        private IComparer comparer;
        private IRater rater;
        private UserProductRatingsTable ratings;

        private int neighborCount;
        private int latentUserFeatureCount;

        private readonly IHostingEnvironment _hostingEnvironment;

        public UserCollaborativeFilterRecommender(IComparer userComparer, IRater implicitRater, IHostingEnvironment hostingEnvironment, int numberOfNeighbors)
            : this(userComparer, implicitRater, hostingEnvironment, numberOfNeighbors, 20)
        {
        }

        public UserCollaborativeFilterRecommender(IComparer userComparer, IRater implicitRater, IHostingEnvironment hostingEnvironment, int numberOfNeighbors, int latentFeatures)
        {
            comparer = userComparer;
            rater = implicitRater;
            neighborCount = numberOfNeighbors;
            latentUserFeatureCount = latentFeatures;
            _hostingEnvironment = hostingEnvironment;
        }

        public bool Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            ratings = ubt.GetUserProductRatingsTable(rater);

            if (latentUserFeatureCount > 0)
            {
                SingularValueDecomposition svd = new SingularValueDecomposition(latentUserFeatureCount, 100);
                SvdResult results = svd.FactorizeMatrix(ratings);

                ratings.AppendUserFeatures(results.UserFeatures);
                return true;
            }
            return false;
        }

        public double GetRating(Guid userId, int articleId)
        {
            UserProductRatings user = ratings.Users.FirstOrDefault(x => x.UserID == userId);
            List<UserProductRatings> neighbors = GetNearestNeighbors(user, neighborCount);

            return GetRating(user, neighbors, articleId);
        }

        private double GetRating(UserProductRatings user, List<UserProductRatings> neighbors, int productId)
        {
            int articleIndex = ratings.ProductIndexToID.IndexOf(productId);

            var nonZero = user.ProductRatings.Where(x => x != 0);
            double avgUserRating = nonZero.Count() > 0 ? nonZero.Average() : 0.0;

            double score = 0.0;
            int count = 0;
            for (int u = 0; u < neighbors.Count; u++)
            {
                var nonZeroRatings = neighbors[u].ProductRatings.Where(x => x != 0);
                double avgRating = nonZeroRatings.Count() > 0 ? nonZeroRatings.Average() : 0.0;

                if (neighbors[u].ProductRatings[articleIndex] != 0)
                {
                    score += neighbors[u].ProductRatings[articleIndex] - avgRating;
                    count++;
                }
            }
            if (count > 0)
            {
                score /= count;
                score += avgUserRating;
            }

            return score;
        }

        public List<Suggestion> GetSuggestions(Guid userId, int numSuggestions)
        {
            int userIndex = ratings.UserIndexToID.IndexOf(userId);
            UserProductRatings user = ratings.Users[userIndex];
            List<Suggestion> suggestions = new List<Suggestion>();

            var neighbors = GetNearestNeighbors(user, neighborCount);

            for (int articleIndex = 0; articleIndex < ratings.ProductIndexToID.Count; articleIndex++)
            {
                // If the user in question hasn't rated the given article yet
                if (user.ProductRatings[articleIndex] == 0)
                {
                    double score = 0.0;
                    int count = 0;
                    for (int u = 0; u < neighbors.Count; u++)
                    {
                        if (neighbors[u].ProductRatings[articleIndex] != 0)
                        {
                            // Calculate the weighted score for this article   
                            score += neighbors[u].ProductRatings[articleIndex] - ((u + 1.0) / 100.0);
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        score /= count;
                    }

                    suggestions.Add(new Suggestion(userId, ratings.ProductIndexToID[articleIndex], score));
                }
            }

            suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));

            return suggestions.Take(numSuggestions).ToList();
        }

        private List<UserProductRatings> GetNearestNeighbors(UserProductRatings user, int numUsers)
        {
            List<UserProductRatings> neighbors = new List<UserProductRatings>();

            for (int i = 0; i < ratings.Users.Count; i++)
            {
                if (ratings.Users[i].UserID == user.UserID)
                {
                    ratings.Users[i].Score = double.NegativeInfinity;
                }
                else
                {
                    ratings.Users[i].Score = comparer.CompareVectors(ratings.Users[i].ProductRatings, user.ProductRatings);
                }
            }

            var similarUsers = ratings.Users.OrderByDescending(x => x.Score);

            return similarUsers.Take(numUsers).ToList();
        }

        public void Save(string file)
        {
            if (!Directory.Exists(_hostingEnvironment.WebRootPath + $@"\uploaded\recommendation\"))
            {
                Directory.CreateDirectory(_hostingEnvironment.WebRootPath + $@"\uploaded\recommendation");
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
        }

        public List<Suggestion> GetSuggestions(Guid userId, int pId, int numSuggestions)
        {
            throw new NotImplementedException();
        }
    }
}
