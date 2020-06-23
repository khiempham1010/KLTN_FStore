using AtomStore.Application.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Interfaces
{
    public interface IProductFeedbackService: IDisposable
    {
        List<ProductFeedbackViewModel> GetAll();
        List<ProductFeedbackViewModel> GetByProductId(int productId);
        ProductFeedbackViewModel Add(ProductFeedbackViewModel feedbackVM);

        void Update(ProductFeedbackViewModel feedbackVM);

        void Delete(int id);

        void Save();
        ProductFeedbackViewModel GetById(int id);
        void Like(int id);
        void DisLike(int id);
        void AddImages(int feedbackId, string[] images);
        List<FeedbackImageViewModel> GetImages(int feedbackId);
    }
}
