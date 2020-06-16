using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Recommender.Algorithms
{
    class Matrix
    {
        /// <summary>
        /// Calculate the dot product between two vectors
        /// </summary>
        public static double GetDotProduct(double[] matrixOne, double[] matrixTwo)
        {
            return matrixOne.Zip(matrixTwo, (a, b) => a * b).Sum();
        }
    }
}
