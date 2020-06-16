using System;
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

namespace AtomStore.Controllers
{
    public class HomeController : Controller
    {
        private IProductService _productService;
        private IProductCategoryService _productCategoryService;
        private IRecommenderService _recommenderService;

        //private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(IProductService productService, IProductCategoryService productCategoryService, IRecommenderService recommenderService)
        {

            _productService = productService;
            _productCategoryService = productCategoryService;
            _recommenderService = recommenderService;
            //_localizer = localizer;
        }

        //[ResponseCache(CacheProfileName = "Default")]
        public IActionResult Index()
        {
            //var title = _localizer["Title"];
            //var culture = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            //ViewData["BodyClass"] = "cms-index-index cms-home-page";
            var homeVm = new HomeViewModel();
            homeVm.HomeCategories = _productCategoryService.GetHomeCategories(8);
            homeVm.HotProducts = _productService.GetHotProduct(10);
            homeVm.TopLatestProducts = _productService.GetLastest(10);
            //homeVm.LastestBlogs = _blogService.GetLastest(5);
            //homeVm.HomeSlides = _commonService.GetSlides("top");
            _recommenderService.TrainData();
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
