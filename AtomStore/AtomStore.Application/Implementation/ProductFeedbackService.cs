using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Entities;
using AtomStore.Infrastructure.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Implementation
{
    public class ProductFeedbackService : IProductFeedbackService
    {
        private readonly IRepository<ProductFeedback,int> _feedbackRepository;
        private IUnitOfWork _unitOfWork;
        IUserService _userService;
        private readonly IRepository<FeedbackImage, int> _feedbackImageRepository;
        public ProductFeedbackService()
        {

        }
        public ProductFeedbackService(IRepository<ProductFeedback, int> feedback, IUnitOfWork unitOfWork, IUserService userService, IRepository<FeedbackImage, int> feedbackImageRepository)
        {
            _feedbackRepository = feedback;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _feedbackImageRepository = feedbackImageRepository;
        }
        public ProductFeedbackViewModel Add(ProductFeedbackViewModel feedbackVM)
        {
            var feedback = Mapper.Map<ProductFeedbackViewModel, ProductFeedback>(feedbackVM);
            _feedbackRepository.Add(feedback);
            return feedbackVM;
        }

        public void Delete(int id)
        {
            _feedbackRepository.Remove(id);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ProductFeedbackViewModel GetById(int id)
        {
            return Mapper.Map<ProductFeedback, ProductFeedbackViewModel>(_feedbackRepository.FindById(id));
        }

        public List<ProductFeedbackViewModel> GetByProductId(int productId)
        {
            var feedbacks = Mapper.Map<List<ProductFeedback>, List<ProductFeedbackViewModel>>(_feedbackRepository.FindAll(x => x.ProductId == productId).OrderByDescending(x=>x.DateCreated).ToList());
            foreach (var item in feedbacks)
            {
                item.AppUser = _userService.GetById(item.OwnerId.ToString()).Result;
            }
            return feedbacks;
        }

        public void Like(int id)
        {
            var feedback = _feedbackRepository.FindById(id);
            feedback.Like += 1;
            _feedbackRepository.Update(feedback);
        }
        public void DisLike(int id)
        {
            var feedback = _feedbackRepository.FindById(id);
            feedback.Like -= 1;
            _feedbackRepository.Update(feedback);
        }

        public void Update(ProductFeedbackViewModel feedbackVM)
        {
            var feedback = _feedbackRepository.FindById(feedbackVM.Id);
            _feedbackRepository.Update(feedback);
        }

        public void AddImages(int feedbackId, string[] images)
        {
            _feedbackImageRepository.RemoveMultiple(_feedbackImageRepository.FindAll(x => x.ProductFeedbackId == feedbackId).ToList());
            foreach (var image in images)
            {
                _feedbackImageRepository.Add(new FeedbackImage()
                {
                    Path = image,
                    ProductFeedbackId = feedbackId,
                    Caption = string.Empty
                });
            }
        }

        public List<FeedbackImageViewModel> GetImages(int feedbackId)
        {
            return _feedbackImageRepository.FindAll(x => x.ProductFeedbackId == feedbackId)
                .ProjectTo<FeedbackImageViewModel>().ToList();
        }
    }
}
