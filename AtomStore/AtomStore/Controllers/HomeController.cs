﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AtomStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Identity;
using AtomStore.Data.Entities;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Utilities.Constants;
using AtomStore.Extensions;

namespace AtomStore.Controllers
{
    public class HomeController : Controller
    {
        private IProductService _productService;
        private IProductCategoryService _productCategoryService;
        private IRecommenderService _recommenderService;
        IWishlistService _wishlistService;
        UserManager<AppUser> _userManager;
        private IProductFeedbackService _productFeedbackService;
        private readonly IVisitorCounterService _visitorCounterService;
        //private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(IProductService productService, IProductCategoryService productCategoryService, IRecommenderService recommenderService, IWishlistService wishlistService,
            UserManager<AppUser> userManager, IProductFeedbackService productFeedbackService
            , IVisitorCounterService visitorCounterService)
        {

            _productService = productService;
            _productCategoryService = productCategoryService;
            _recommenderService = recommenderService;
            _wishlistService = wishlistService;
            _userManager = userManager;
            _productFeedbackService = productFeedbackService;
            _visitorCounterService = visitorCounterService;
            
            //_localizer = localizer;
        }

        //[ResponseCache(CacheProfileName = "Default")]
        public IActionResult Index()
        {
            var visitorId = HttpContext.Session.Get<string>(CommonConstants.visitorId);
            if (visitorId == null)
            {
                //don the necessary staffs here to save the count by one
                HttpContext.Session.Set(CommonConstants.visitorId,Guid.NewGuid().ToString());
                _visitorCounterService.SetVisitors();
            }

            //var title = _localizer["Title"];
            //var culture = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            //ViewData["BodyClass"] = "cms-index-index cms-home-page";
            _recommenderService.TrainData();
            var homeVm = new HomeViewModel();
            var currentUser = _userManager.GetUserAsync(User).Result;
            homeVm.HomeCategories = _productCategoryService.GetHomeCategories(8);
            homeVm.TopLatestProducts = _productService.GetLastest(10);
            homeVm.HotProducts = _productService.GetBestSellingProduct(10);
            if (currentUser != null)
            {
                foreach (var item in homeVm.HotProducts)
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
                foreach (var item in homeVm.TopLatestProducts)
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
                homeVm.RecommendProduct = recommendProducts;
            }
            //homeVm.LastestBlogs = _blogService.GetLastest(5);
            //homeVm.HomeSlides = _commonService.GetSlides("top");
            return View(homeVm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
