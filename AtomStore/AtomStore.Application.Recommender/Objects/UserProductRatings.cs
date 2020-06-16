using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Objects
{
    public class UserProductRatings
    {
        public Guid UserID { get; set; }

        public double[] ProductRatings { get; set; }

        public double Score { get; set; }

        public UserProductRatings(Guid userId, int ProductCount)
        {
            UserID = userId;
            ProductRatings = new double[ProductCount];
        }

        public void AppendRatings(double[] ratings)
        {
            List<double> allRatings = new List<double>();

            allRatings.AddRange(ProductRatings);
            allRatings.AddRange(ratings);

            ProductRatings = allRatings.ToArray();
        }
    }
}
