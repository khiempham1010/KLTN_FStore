using AtomStore.Application.Recommender.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Interfaces
{
    public interface IRater
    {
        double GetRating(List<UserAction> actions);
    }
}
