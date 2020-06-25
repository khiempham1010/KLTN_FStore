using AtomStore.Application.Interfaces;
using AtomStore.Application.Recommender.Comparers;
using AtomStore.Application.Recommender.Interfaces;
using AtomStore.Application.Recommender.Parsers;
using AtomStore.Application.Recommender.Raters;
using AtomStore.Application.Recommender.Recommender;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Entities;
using AtomStore.Infrastructure.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AtomStore.Application.Implementation
{
    public class RecommenderService : IRecommenderService
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly UserManager<AppUser> _userManager;
        private IRepository<Tag, string> _tagRepository;
        private readonly IRepository<ViewedList, int> _viewedlistRepository;
        private readonly IRepository<WishList, int> _wishlistRepository;
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<ProductFeedback, int> _feedbackRepository;
        private readonly IRepository<OrderDetail, int> _orderDetailRepository;
        UserBehaviorDatabaseParser parser;
        IRecommender recommender;
        private readonly IHostingEnvironment _hostingEnvironment;

        string savedModel =$@"\uploaded\recommendation\recommender.dat";
        public RecommenderService(
            IRepository<Product, int> productRepository,
            UserManager<AppUser> userManager,
            IRepository<Tag, string> tagRepository,
            IRepository<ViewedList, int> viewedlistRepository,
            IRepository<WishList, int> wishlistRepository,
            IRepository<Order, int> orderRepository,
            IRepository<ProductFeedback, int> feedbackRepository, 
            IRepository<OrderDetail, int> orderDetailRepository,
            IHostingEnvironment hostingEnvironment)
        {
            _productRepository = productRepository;
            _userManager = userManager;
            _tagRepository = tagRepository;
            _viewedlistRepository = viewedlistRepository;
            _wishlistRepository = wishlistRepository;
            _orderRepository = orderRepository;
            _feedbackRepository = feedbackRepository;
            _orderDetailRepository = orderDetailRepository;
            _hostingEnvironment = hostingEnvironment;
            parser = new UserBehaviorDatabaseParser(_productRepository, _userManager, _tagRepository, _viewedlistRepository, _wishlistRepository, _orderRepository, _feedbackRepository,_orderDetailRepository);

            IRater rate = new LinearRater(2, 3, 4, 5, -3);
            IComparer compare = new CorrelationUserComparer();

            //user base
            recommender = new UserCollaborativeFilterRecommender(compare, rate, _hostingEnvironment, 5);
            if (File.Exists(_hostingEnvironment.WebRootPath + savedModel))
            {
                try
                {
                    recommender.Load(_hostingEnvironment.WebRootPath + savedModel);
                }
                catch (Exception e)
                {
                    var error = e.StackTrace;
                }
            }

            //item base
            //recommender = new ItemCollaborativeFilterRecommender(compare, rate, 30);
            //if (File.Exists(savedModel))
            //{
            //    try
            //    {
            //        recommender.Load(savedModel);
            //    }
            //    catch (Exception e)
            //    {
            //        var error = e.StackTrace;
            //    }
            //}
        }
        public List<ProductViewModel> GetRecommendProduct(Guid userId, int numSuggestion)
        {
            var listSuggestion = recommender.GetSuggestions(userId, numSuggestion);
            var listProduct = new List<ProductViewModel>();
            foreach(var item in listSuggestion)
            {
                listProduct.Add(Mapper.Map<Product, ProductViewModel>(_productRepository.FindById(item.ProductID)));
            }
            return listProduct;
        }
        public List<ProductViewModel> GetRecommendProduct(Guid userId,int pId , int numSuggestion)
        {
            var listSuggestion = recommender.GetSuggestions(userId, pId, numSuggestion);
            var listProduct = new List<ProductViewModel>();
            foreach (var item in listSuggestion)
            {
                listProduct.Add(Mapper.Map<Product, ProductViewModel>(_productRepository.FindById(item.ProductID)));
            }
            return listProduct;
        }
        public void TrainData()
        {
            UserBehaviorDatabase db = parser.LoadUserBehaviorDatabase();
            recommender.Train(db);
            recommender.Save(_hostingEnvironment.WebRootPath + savedModel);    
        }
    }
}
