using AtomStore.Application.Recommender.Interfaces;
using AtomStore.Application.Recommender.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Recommender.Raters
{
    public class LinearRater : IRater
    {
        private double viewWeight;
        private double addToWishlistWeight;
        private double buyWeight;
        private double goodRatingWeight;
        private double badRatingWeight;

        private double minWeight;
        private double maxWeight;

        public LinearRater()
            : this(1.0, 3.0, 4.0, 5.0, -3.0,5.0)
        {
        }

        public LinearRater(double view, double addToWishlist, double buy, double goodRating, double badRating)
            : this(view, addToWishlist, buy, goodRating, badRating, 5.0)
        {
        }

        public LinearRater(double view, double addToWishlist, double buy, double goodRating, double badRating, double max)

        {
            viewWeight = view;
            addToWishlistWeight = addToWishlist;
            buyWeight = buy;
            goodRatingWeight = goodRating;
            badRatingWeight = badRating;

            minWeight = 0.1;
            maxWeight = max;
        }
        public double GetRating(List<UserAction> actions)
        {
            int vi = actions.Count(x => x.Action == "view");
            int atw = actions.Count(x => x.Action == "add_to_wishlist");
            int buy = actions.Count(x => x.Action == "buy");
            int gr = actions.Count(x => x.Action == "good_rating");
            int br = actions.Count(x => x.Action == "bad_rating");

            double rating = vi * viewWeight + atw * addToWishlistWeight +
                buy * buyWeight + gr * goodRatingWeight + br*badRatingWeight;

            return Math.Min(maxWeight, Math.Max(minWeight, rating));
        }
    }
}
