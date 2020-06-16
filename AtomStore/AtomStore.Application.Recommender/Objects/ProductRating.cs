using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Objects
{
    public class ProductRating
    {
        public int ProductID { get; set; }

        public double Rating { get; set; }

        public ProductRating(int ProductId, double rating)
        {
            ProductID = ProductId;
            Rating = rating;
        }
    }
}
