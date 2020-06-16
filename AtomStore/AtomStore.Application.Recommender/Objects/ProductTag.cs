using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Objects
{
    public class ProductTag
    {
        public ProductTag(int productid, string tagId)
        {
            ProductId = productid;
            TagId = tagId;
        }
        public int ProductId { get; set; }
        public string TagId { get; set; }
    }
}
