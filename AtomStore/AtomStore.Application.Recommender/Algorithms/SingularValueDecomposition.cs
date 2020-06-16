using AtomStore.Application.Recommender.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Algorithms
{
    class SingularValueDecomposition
    {
        private double averageGlobalRating;

        private int learningIterations;
        private double learningRate;
        private double learningDescent = 0.99;
        private double regularizationTerm = 0.02;

        private int numUsers;
        private int numProduct;
        private int numFeatures;

        private double[] userBiases;
        private double[] articleBiases;
        private double[][] userFeatures;
        private double[][] articleFeatures;

        public SingularValueDecomposition()
            : this(20, 100)
        {
        }

        public SingularValueDecomposition(int features, int iterations)
            : this(features, iterations, 0.005)
        {
        }

        public SingularValueDecomposition(int features, int iterations, double learningSpeed)
        {
            numFeatures = features;
            learningIterations = iterations;
            learningRate = learningSpeed;
        }

        private void Initialize(UserProductRatingsTable ratings)
        {
            numUsers = ratings.Users.Count;
            numProduct = ratings.Users[0].ProductRatings.Length;

            Random rand = new Random();

            userFeatures = new double[numUsers][];
            for (int userIndex = 0; userIndex < numUsers; userIndex++)
            {
                userFeatures[userIndex] = new double[numFeatures];

                for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                {
                    userFeatures[userIndex][featureIndex] = rand.NextDouble();
                }
            }

            articleFeatures = new double[numProduct][];
            for (int articleIndex = 0; articleIndex < numProduct; articleIndex++)
            {
                articleFeatures[articleIndex] = new double[numFeatures];

                for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                {
                    articleFeatures[articleIndex][featureIndex] = rand.NextDouble();
                }
            }

            userBiases = new double[numUsers];
            articleBiases = new double[numProduct];
        }

        public SvdResult FactorizeMatrix(UserProductRatingsTable ratings)
        {
            Initialize(ratings);

            double squaredError;
            int count;
            List<double> rmseAll = new List<double>();

            averageGlobalRating = GetAverageRating(ratings);

            for (int i = 0; i < learningIterations; i++)
            {
                squaredError = 0.0;
                count = 0;

                for (int userIndex = 0; userIndex < numUsers; userIndex++)
                {
                    for (int articleIndex = 0; articleIndex < numProduct; articleIndex++)
                    {
                        if (ratings.Users[userIndex].ProductRatings[articleIndex] != 0)
                        {
                            double predictedRating = averageGlobalRating + userBiases[userIndex] + articleBiases[articleIndex] + Matrix.GetDotProduct(userFeatures[userIndex], articleFeatures[articleIndex]);

                            double error = ratings.Users[userIndex].ProductRatings[articleIndex] - predictedRating;

                            if (double.IsNaN(predictedRating))
                            {
                                throw new Exception("Encountered a non-number while factorizing a matrix! Try decreasing the learning rate.");
                            }

                            squaredError += Math.Pow(error, 2);
                            count++;

                            averageGlobalRating += learningRate * (error - regularizationTerm * averageGlobalRating);
                            userBiases[userIndex] += learningRate * (error - regularizationTerm * userBiases[userIndex]);
                            articleBiases[articleIndex] += learningRate * (error - regularizationTerm * articleBiases[articleIndex]);

                            for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                            {
                                userFeatures[userIndex][featureIndex] += learningRate * (error * articleFeatures[articleIndex][featureIndex] - regularizationTerm * userFeatures[userIndex][featureIndex]);
                                articleFeatures[articleIndex][featureIndex] += learningRate * (error * userFeatures[userIndex][featureIndex] - regularizationTerm * articleFeatures[articleIndex][featureIndex]);
                            }
                        }
                    }
                }

                squaredError = Math.Sqrt(squaredError / count);
                rmseAll.Add(squaredError);

                learningRate *= learningDescent;
            }

            //using (StreamWriter w = new StreamWriter("rmse.csv"))
            //{
            //    w.WriteLine("epoc,rmse");
            //    for (int i = 0; i < rmseAll.Count; i++)
            //    {
            //        w.WriteLine((i + 1) + "," + rmseAll[i]);
            //    }
            //}

            return new SvdResult(averageGlobalRating, userBiases, articleBiases, userFeatures, articleFeatures);
        }

        /// <summary>
        /// Get the average rating of non-zero values across the entire user-article matrix
        /// </summary>
        private double GetAverageRating(UserProductRatingsTable ratings)
        {
            double sum = 0.0;
            int count = 0;

            for (int userIndex = 0; userIndex < numUsers; userIndex++)
            {
                for (int articleIndex = 0; articleIndex < numProduct; articleIndex++)
                {
                    // If the given user rated the given item, add it to our average
                    if (ratings.Users[userIndex].ProductRatings[articleIndex] != 0)
                    {
                        sum += ratings.Users[userIndex].ProductRatings[articleIndex];
                        count++;
                    }
                }
            }

            return sum / count;
        }
    }
}
