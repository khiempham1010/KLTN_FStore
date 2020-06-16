using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Algorithms
{
    class SvdResult
    {
        public double AverageGlobalRating { get; private set; }
        public double[] UserBiases { get; private set; }
        public double[] ProductBiases { get; private set; }
        public double[][] UserFeatures { get; private set; }
        public double[][] ProductFeatures { get; private set; }

        public SvdResult(double averageGlobalRating, double[] userBiases, double[] productBiases, double[][] userFeatures, double[][] productFeatures)
        {
            AverageGlobalRating = averageGlobalRating;
            UserBiases = userBiases;
            ProductBiases = productBiases;
            UserFeatures = userFeatures;
            ProductFeatures = productFeatures;
        }
    }
}
