﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Entities;
using AtomStore.Models;
using AtomStore.Models.ProductViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace AtomStore.Controllers
{
    public class ProductController : Controller
    {
        IProductService _productService;
        IOrderService _orderService;
        IProductCategoryService _productCategoryService;
        IConfiguration _configuration;
        IWishlistService _wishlistService;
        IViewedlistService _viewedlistService;
        public readonly UserManager<AppUser> _userManager;
        IProductFeedbackService _productFeedbackService;
        IUserService _userService;
        IRecommenderService _recommenderService;
        public ProductController(IProductService productService, IConfiguration configuration,
            IOrderService orderService,
            IProductCategoryService productCategoryService,
            IWishlistService wishlistService,
            IViewedlistService viewedlistService,
            UserManager<AppUser> userManager,
            IProductFeedbackService productFeedbackService,
            IUserService userService,
            IRecommenderService recommenderService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _configuration = configuration;
            _orderService = orderService;
            _wishlistService = wishlistService;
            _viewedlistService = viewedlistService;
            _userManager = userManager;
            _productFeedbackService = productFeedbackService;
            _userService = userService;
            _recommenderService = recommenderService;
        }
        [Route("products.html")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("{alias}-c.{id}.html")]
        public IActionResult Catalog(int id, int? pageSize, string sortBy, int page = 1)
        {
            
            var catalog = new CatalogViewModel();
            if (pageSize == null)
                pageSize = 12;
            //pageSize = _configuration.GetValue<int>("PageSize");

            catalog.PageSize = pageSize;
            catalog.SortType = sortBy;
            if (!string.IsNullOrEmpty(HttpContext.Request.Query["minPrice"]))
            {
                var queryString = HttpContext.Request.Query;
                StringValues minPriceString;
                queryString.TryGetValue("minPrice", out minPriceString);
                int minPrice = int.Parse(minPriceString);
                StringValues maxPriceString;
                queryString.TryGetValue("maxPrice", out maxPriceString);
                int maxPrice = int.Parse(maxPriceString);
                catalog.Data = _productService.GetAllPaging(id, minPrice, maxPrice, string.Empty, page, pageSize.Value,sortBy);
            }
            else
            {
                catalog.Data = _productService.GetAllPaging(id, null, null, string.Empty, page, pageSize.Value,sortBy);
            }
            catalog.Category = _productCategoryService.GetById(id);
            var currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser != null)
            {
                foreach (var item in catalog.Data.Results)
                {
                    item.Wishlist = _wishlistService.GetByProductAndUserId(item.Id, currentUser.Id) == default ? false : true;
                }
            }
            return View(catalog);
        }

        [Route("search.html")]
        public IActionResult Search(string keyword, int? pageSize, string sortBy, int page = 1)
        {
            var catalog = new SearchResultViewModel();
            ViewData["BodyClass"] = "shop_grid_full_width_page";
            if (pageSize == null)
                pageSize = 12;

            catalog.PageSize = pageSize;
            catalog.SortType = sortBy;
            catalog.Data = _productService.GetAllPaging(null, null, null, keyword, page, pageSize.Value,sortBy);
            catalog.Keyword = keyword;
            var currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser != null)
            {
                foreach (var item in catalog.Data.Results)
                {
                    item.Wishlist = _wishlistService.GetByProductAndUserId(item.Id, currentUser.Id) == default ? false : true;
                }
            }
            return View(catalog);
        }

        [Route("{alias}-p.{id}.html", Name = "ProductDetail")]
        public IActionResult Details(int id)
        {
            _recommenderService.TrainData();
            var model = new DetailViewModel();
            model.Product = _productService.GetById(id);
            var currentUser = _userManager.GetUserAsync(User).Result;
            model.Category = _productCategoryService.GetById(model.Product.CategoryId);
            model.RelatedProducts = _productService.GetRelatedProducts(id, 9);
            if (currentUser != null)
            {
                foreach (var item in model.RelatedProducts)
                {
                    item.Wishlist = _wishlistService.GetByProductAndUserId(item.Id, currentUser.Id) == default ? false : true;
                }
            }
            model.UpsellProducts = _productService.GetUpsellProducts(6);
            model.ProductImages = _productService.GetImages(id);
            model.Tags = _productService.GetProductTags(id);
            model.Colors = _productService.GetAvailableColor(id).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            model.Sizes = _productService.GetAvailableSize(id).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            
            if (currentUser != null)
            {
                var wishlist = _wishlistService.GetByProductAndUserId(model.Product.Id, currentUser.Id);
                model.Wishlist = wishlist;
            }

            var viewedList = new ViewedlistViewModel();
            if (currentUser != null)
            {
                var viewed = _viewedlistService.GetByProductAndUserId(id, currentUser.Id);
                var viewList = new List<ViewedlistViewModel>();
                if (viewed != null)
                {
                    viewList = _viewedlistService.GetAll(currentUser.Id);
                    foreach (var item in viewList)
                    {
                        item.Product.Wishlist = _wishlistService.GetByProductAndUserId(item.ProductId, currentUser.Id) == default ? false : true;
                        var fb = new List<ProductFeedbackViewModel>();
                        fb = _productFeedbackService.GetByProductId(item.ProductId);
                        item.Product.Rating = 0;
                        if (fb.Count > 0)
                        {
                            item.Product.Rating = (int)fb.Select(x => x.Rating).Average();
                        }
                    }
                    model.Viewedlist = viewList;
                }
                else
                {
                    viewedList.ProductId = id;
                    viewedList.Product = _productService.GetById(id);
                    viewedList.UserId = currentUser.Id;
                    viewedList.ProductName = viewedList.Product.Name;
                    viewedList.Email = currentUser.Email;
                    _viewedlistService.Create(viewedList);
                    _viewedlistService.Save();
                    viewList = _viewedlistService.GetAll(currentUser.Id);
                    foreach (var item in viewList)
                    {
                        item.Product.Wishlist = _wishlistService.GetByProductAndUserId(item.ProductId, currentUser.Id) == default ? false : true;
                        var fb = new List<ProductFeedbackViewModel>();
                        fb = _productFeedbackService.GetByProductId(item.Id);
                        item.Product.Rating = 0;
                        if (fb.Count > 0)
                        {
                            item.Product.Rating = (int)fb.Select(x => x.Rating).Average();
                        }
                    }
                    model.Viewedlist = viewList;
                }

            }

            var feedbacks = new List<ProductFeedbackViewModel>();
            feedbacks = _productFeedbackService.GetByProductId(id);
            model.ProductFeedback = feedbacks;
            model.Rating = 0;
            if (feedbacks.Count > 0)
            {
                model.Rating = (int)feedbacks.Select(x => x.Rating).Average();
            }
            if (currentUser != null)
            {
                var recommendProducts = _recommenderService.GetRecommendProduct(currentUser.Id, 10);
                foreach (var item in recommendProducts)
                {
                    item.Wishlist = _wishlistService.GetByProductAndUserId(item.Id, currentUser.Id) == default ? false : true;
                    var fb = new List<ProductFeedbackViewModel>();
                    fb = _productFeedbackService.GetByProductId(item.Id);
                    item.Rating = 0;
                    if (fb.Count > 0)
                    {
                        item.Rating = (int)fb.Select(x => x.Rating).Average();
                    }
                }
                model.recommendProducts = recommendProducts;
            }

            return View(model);

        }
        [HttpPost]
        public IActionResult AddWishlist(int productId)
        {
            var currentUser = _userManager.GetUserAsync(User).Result;

            var wishList = new WishlistViewModel();
            if (currentUser != null)
            {
                var wish = _wishlistService.GetByProductAndUserId(productId, currentUser.Id);
                if (wish != null)
                {
                    _wishlistService.Delete(wish.Id);
                    _wishlistService.Save();
                    return new OkObjectResult(productId);
                }
                else
                {
                    wishList.ProductId = productId;
                    wishList.Product = _productService.GetById(productId);
                    wishList.UserId = currentUser.Id;
                    wishList.ProductName = wishList.Product.Name;
                    wishList.Email = currentUser.Email;
                    _wishlistService.Create(wishList);
                    _wishlistService.Save();
                    return new OkObjectResult(productId);
                }
            }
            else
            {
                return new BadRequestObjectResult(currentUser);
            }

        }

        [HttpPost]
        public IActionResult AddViewedlist(int productId)
        {
            var currentUser = _userManager.GetUserAsync(User).Result;

            var viewedList = new ViewedlistViewModel();
            if (currentUser != null)
            {
                var viewed = _viewedlistService.GetByProductAndUserId(productId, currentUser.Id);
                if (viewed != null)
                {
                    _viewedlistService.Delete(viewed.Id);
                    _viewedlistService.Save();
                    return new OkObjectResult(productId);
                }
                else
                {
                    viewedList.ProductId = productId;
                    viewedList.Product = _productService.GetById(productId);
                    viewedList.UserId = currentUser.Id;
                    viewedList.ProductName = viewedList.Product.Name;
                    viewedList.Email = currentUser.Email;
                    _viewedlistService.Create(viewedList);
                    _viewedlistService.Save();
                    return new OkObjectResult(productId);
                }
            }
            else
            {
                return new BadRequestObjectResult(currentUser);
            }
        }

        [HttpPost]
        public IActionResult AddFeedback(ProductFeedbackViewModel feedbackVM)
        {
            if (ModelState != null)
            {
                var currentUser = _userManager.GetUserAsync(User).Result;

                if (currentUser != null)
                {
                    feedbackVM.OwnerId = currentUser.Id;

                    _productFeedbackService.Add(feedbackVM);
                    _productFeedbackService.Save();
                    return new OkObjectResult(feedbackVM);
                }
            }
            return new BadRequestObjectResult(feedbackVM);
        }
        [HttpGet]
        public IActionResult GetFeedback(int productId)
        {
            var feedbacks = new List<ProductFeedbackViewModel>();
            feedbacks = _productFeedbackService.GetByProductId(productId);
            return new OkObjectResult(feedbacks);

        }
        [HttpPost]
        public IActionResult SaveFeedbackImages(int productId, string[] images)
        {
            _productFeedbackService.AddImages(productId, images);
            _productService.Save();
            return new OkObjectResult(images);
        }
        [HttpGet]
        public IActionResult GetImages(int productId)
        {
            var images = _productFeedbackService.GetImages(productId);
            return new OkObjectResult(images);
        }

        [HttpGet]
        public IActionResult GetQuantity(int productId, int colorId, int sizeId)
        {
            return new OkObjectResult(_productService.CheckAvailability(productId,sizeId,colorId));
        }


    }
}