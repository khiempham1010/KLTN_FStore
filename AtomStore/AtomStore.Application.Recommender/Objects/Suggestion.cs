using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Objects
{
    public class Suggestion
    {
        public Guid UserID { get; set; }

        public int ProductID { get; set; }

        public double Rating { get; set; }

        public Suggestion(Guid userId, int productId, double assurance)
        {
            UserID = userId;
            ProductID = productId;
            Rating = assurance;
        }
    }
}
