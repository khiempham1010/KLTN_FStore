using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Objects
{
    public class ProductTagCounts
    {
        public int ProductID { get; set; }

        public double[] TagCounts { get; set; }

        public ProductTagCounts(int productId, int numTags)
        {
            ProductID = productId;
            TagCounts = new double[numTags];
        }
    }
}
