using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Objects
{
    public class UserProductRatingsTable
    {
        public List<UserProductRatings> Users { get; set; }

        public List<Guid> UserIndexToID { get; set; }

        public List<int> ProductIndexToID { get; set; }

        public UserProductRatingsTable()
        {
            Users = new List<UserProductRatings>();
            UserIndexToID = new List<Guid>();
            ProductIndexToID = new List<int>();
        }

        public void AppendUserFeatures(double[][] userFeatures)
        {
            for (int i = 0; i < UserIndexToID.Count; i++)
            {
                Users[i].AppendRatings(userFeatures[i]);
            }
        }

        public void AppendProductFeatures(double[][] productFeatures)
        {
            for (int f = 0; f < productFeatures[0].Length; f++)
            {
                UserProductRatings newFeature = new UserProductRatings(Guid.NewGuid(), ProductIndexToID.Count);

                for (int a = 0; a < ProductIndexToID.Count; a++)
                {
                    newFeature.ProductRatings[a] = productFeatures[a][f];
                }

                Users.Add(newFeature);
            }
        }

        internal void AppendProductFeatures(List<ProductTagCounts> productTags)
        {
            double[][] features = new double[productTags.Count][];

            for (int a = 0; a < productTags.Count; a++)
            {
                features[a] = new double[productTags[a].TagCounts.Length];

                for (int f = 0; f < productTags[a].TagCounts.Length; f++)
                {
                    features[a][f] = productTags[a].TagCounts[f];
                }
            }

            AppendProductFeatures(features);
        }
    }
}
