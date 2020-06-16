using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Interfaces
{
    public interface IRecommenderService
    {
        List<ProductViewModel> GetRecommendProduct(Guid userId, int numSuggestion);
        List<ProductViewModel> GetRecommendProduct(Guid userId, int pId, int numSuggestion);
        void TrainData();
    }
}
