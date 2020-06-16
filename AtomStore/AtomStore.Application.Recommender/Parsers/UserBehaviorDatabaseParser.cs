using AtomStore.Data.Entities;
using AtomStore.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Recommender.Parsers
{
    public class UserBehaviorDatabaseParser
    {
        private readonly IRepository<Product, int> _productRepository;
        private readonly UserManager<AppUser> _userManager;
        private IRepository<Tag, string> _tagRepository;
        private readonly IRepository<ViewedList, int> _viewedlistRepository;
        private readonly IRepository<WishList, int> _wishlistRepository;
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<ProductFeedback, int> _feedbackRepository;

        private readonly IRepository<OrderDetail, int> _orderDetailRepository;

        public UserBehaviorDatabaseParser(
            IRepository<Product, int> productRepository,
            UserManager<AppUser> userManager,
            IRepository<Tag, string> tagRepository,
            IRepository<ViewedList, int> viewedlistRepository,
            IRepository<WishList, int> wishlistRepository,
            IRepository<Order, int> orderRepository,
            IRepository<ProductFeedback, int> feedbackRepository,
            IRepository<OrderDetail, int> orderDetailRepository
            )
        {
            _productRepository = productRepository;
            _userManager = userManager;
            _tagRepository = tagRepository;
            _viewedlistRepository = viewedlistRepository;
            _wishlistRepository = wishlistRepository;
            _orderRepository = orderRepository;
            _feedbackRepository = feedbackRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public UserBehaviorDatabase LoadUserBehaviorDatabase()
        {
            UserBehaviorDatabase db = new UserBehaviorDatabase();

            foreach (var tag in _tagRepository.FindAll())
            {
                db.Tags.Add(tag.Name);
            }



            // Parse out the articles
            foreach (var product in _productRepository.FindAll())
            {
                db.Products.Add(product);

            }

            // Parse out the users
            foreach (var user in _userManager.Users)
            {
                db.Users.Add(user);

            }

            // Parse out the user actions
            foreach(var view in _viewedlistRepository.FindAll())
            {
                db.UserActions.Add(new Objects.UserAction(1, "view", view.UserId, view.ProductId));
            }
            foreach (var wish in _wishlistRepository.FindAll())
            {
                db.UserActions.Add(new Objects.UserAction(1, "add_to_wishlist", wish.UserId, wish.ProductId));
            }
            foreach (var buy in _orderRepository.FindAll(x=>x.CustomerId!=null))
            {
                var OrderDetail = _orderDetailRepository.FindAll(x => x.OrderId == buy.Id);
                foreach (var item in OrderDetail)
                {
                    db.UserActions.Add(new Objects.UserAction(1, "buy", buy.CustomerId.Value, item.ProductId));
                }
            }
            foreach (var badRating in _feedbackRepository.FindAll(x => x.Rating < 3)) 
            {
                db.UserActions.Add(new Objects.UserAction(1, "bad_rating", badRating.OwnerId, badRating.ProductId));
            }
            foreach (var goodRating in _feedbackRepository.FindAll(x => x.Rating > 2))
            {
                db.UserActions.Add(new Objects.UserAction(1, "good_rating", goodRating.OwnerId, goodRating.ProductId));
            }

            return db;
        }
    }
}
