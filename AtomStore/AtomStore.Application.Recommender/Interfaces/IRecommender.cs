using AtomStore.Application.Recommender.Objects;
using AtomStore.Application.Recommender.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Interfaces
{
    public interface IRecommender
    {
        bool Train(UserBehaviorDatabase db);

        List<Suggestion> GetSuggestions(Guid userId, int numSuggestions);
        List<Suggestion> GetSuggestions(Guid userId, int pId, int numSuggestions);

        double GetRating(Guid userId, int ProductIdId);

        void Save(string file);

        void Load(string file);
    }
}
