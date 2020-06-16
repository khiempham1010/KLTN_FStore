using AtomStore.Application.Recommender.Interfaces;
using AtomStore.Application.Recommender.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AtomStore.Application.Recommender.Parsers
{
    public class UserBehaviorTransformer
    {
        private UserBehaviorDatabase db;

        public UserBehaviorTransformer(UserBehaviorDatabase database)
        {
            db = database;
        }

        /// <summary>
        /// Get a list of all users and their ratings on every Product
        /// </summary>
        public UserProductRatingsTable GetUserProductRatingsTable(IRater rater)
        {
            UserProductRatingsTable table = new UserProductRatingsTable();

            table.UserIndexToID = db.Users.OrderBy(x => x.Id).Select(x => x.Id).Distinct().ToList();
            table.ProductIndexToID = db.Products.OrderBy(x => x.Id).Select(x => x.Id).Distinct().ToList();

            foreach (Guid userId in table.UserIndexToID)
            {
                table.Users.Add(new UserProductRatings(userId, table.ProductIndexToID.Count));
            }

            var userProductRatingGroup = db.UserActions
                .GroupBy(x => new { x.UserID, x.ProductID })
                .Select(g => new { g.Key.UserID, g.Key.ProductID, Rating = rater.GetRating(g.ToList()) })
                .ToList();

            foreach (var userAction in userProductRatingGroup)
            {
                int userIndex = table.UserIndexToID.IndexOf(userAction.UserID);
                int ProductIndex = table.ProductIndexToID.IndexOf(userAction.ProductID);

                table.Users[userIndex].ProductRatings[ProductIndex] = userAction.Rating;
            }

            return table;
        }

        /// <summary>
        /// Get a table of all Products as rows and all tags as columns
        /// </summary>
        public List<ProductTagCounts> GetProductTagCounts()
        {
            List<ProductTagCounts> ProductTags = new List<ProductTagCounts>();

            foreach (var Product in db.Products)
            {
                ProductTagCounts ProductTag = new ProductTagCounts(Product.Id, db.Tags.Count);

                for (int tag = 0; tag < db.Tags.Count; tag++)
                {
                    ProductTag.TagCounts[tag] = Product.Tags != null && Product.Tags.Any(x=> x.ToString() == db.Tags[tag])? 1.0 : 0.0;
                }

                ProductTags.Add(ProductTag);
            }

            return ProductTags;
        }

        /// <summary>
        /// Get a list of all users and the number of times they viewed Products with a specific tag
        /// </summary>
        //[Obsolete]
        //public List<UserActionTag> GetUserTags()
        //{
        //    List<UserActionTag> userData = new List<UserActionTag>();
        //    List<ProductTag> ProductTags = db.GetProductTagLinkingTable();

        //    int numFeatures = db.Tags.Count;

        //    // Create a dataset that links every user action to a list of tags associated with the Product that action was for, then 
        //    // group them by user, action, and tag so we can get a count of the number of times each user performed a action on an Product with a specific tag
        //    var userActionTags = db.UserActions
        //        .Join(ProductTags, u => u.ProductID, t => t.ProductID, (u, t) => new { u.UserID, t.TagName })
        //        .GroupBy(x => new { x.UserID, x.TagName })
        //        .Select(g => new { g.Key.UserID, g.Key.TagName, Count = g.Count() })
        //        .OrderBy(x => x.UserID).ThenBy(x => x.TagName)
        //        .ToList();

        //    int totalUserActions = userActionTags.Count();
        //    int lastFoundIndex = 0;

        //    // Get action-tag data
        //    foreach (User user in db.Users)
        //    {
        //        int dataCol = 0;
        //        UserActionTag uat = new UserActionTag(user.UserID, numFeatures);

        //        foreach (Tag tag in db.Tags)
        //        {
        //            // Count the number of times this user interacted with an Product with this tag
        //            // We can loop through like this since the list is sorted
        //            int tagActionCount = 0;
        //            for (int i = lastFoundIndex; i < totalUserActions; i++)
        //            {
        //                if (userActionTags[i].UserID == user.UserID && userActionTags[i].TagName == tag.Name)
        //                {
        //                    lastFoundIndex = i;
        //                    tagActionCount = userActionTags[i].Count;
        //                    break;
        //                }
        //            }

        //            uat.ActionTagData[dataCol++] = tagActionCount;
        //        }

        //        // Normalize data to values between 0 and 5
        //        double upperLimit = 5.0;
        //        double max = uat.ActionTagData.Max();
        //        if (max > 0)
        //        {
        //            for (int i = 0; i < uat.ActionTagData.Length; i++)
        //            {
        //                uat.ActionTagData[i] = (uat.ActionTagData[i] / max) * upperLimit;
        //            }
        //        }

        //        userData.Add(uat);
        //    }

        //    return userData;
        //}
    }
}
