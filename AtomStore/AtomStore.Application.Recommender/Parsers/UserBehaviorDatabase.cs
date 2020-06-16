using AtomStore.Application.Recommender.Objects;
using AtomStore.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ProductTag = AtomStore.Application.Recommender.Objects.ProductTag;

namespace AtomStore.Application.Recommender.Parsers

{
    [Serializable]
    public class UserBehaviorDatabase
    {
        public List<string> Tags { get; set; }

        public List<Product> Products { get; set; }

        public List<AppUser> Users { get; set; }

        public List<UserAction> UserActions { get; set; }

        public UserBehaviorDatabase()
        {
            Tags = new List<string>();
            Products = new List<Product>();
            Users = new List<AppUser>();
            UserActions = new List<UserAction>();
        }

        public List<ProductTag> GetArticleTagLinkingTable()
        {
            List<ProductTag> productTags = new List<ProductTag>();

            foreach (Product product in Products)
            {
                productTags.Add(new ProductTag(product.Id, product.Tags));
            }

            return productTags;
        }

        public UserBehaviorDatabase Clone()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;
                return (UserBehaviorDatabase)formatter.Deserialize(ms);
            }
        }
    }
}
