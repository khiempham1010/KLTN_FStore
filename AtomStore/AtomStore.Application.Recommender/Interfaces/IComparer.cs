using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Interfaces
{
    public interface IComparer
    {
        double CompareVectors(double[] userFeaturesOne, double[] userFeaturesTwo);
    }
}
