using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Interfaces;
using AtomStore.Models;
using AtomStore.Models.ProductViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public ProductController(IProductService productService, IConfiguration configuration,
            IOrderService orderService,
            IProductCategoryService productCategoryService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _configuration = configuration;
            _orderService = orderService;
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
                catalog.Data = _productService.GetAllPaging(id, minPrice, maxPrice, string.Empty, page, pageSize.Value);
            }
            else
            {
                catalog.Data = _productService.GetAllPaging(id, null, null, string.Empty, page, pageSize.Value);
            }
            catalog.Category = _productCategoryService.GetById(id);
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
            catalog.Data = _productService.GetAllPaging(null, null, null, keyword, page, pageSize.Value);
            catalog.Keyword = keyword;

            return View(catalog);
        }

        [Route("{alias}-p.{id}.html", Name = "ProductDetail")]
        public IActionResult Details(int id)
        {
            var model = new DetailViewModel();
            model.Product = _productService.GetById(id);
            model.Category = _productCategoryService.GetById(model.Product.CategoryId);
            model.RelatedProducts = _productService.GetRelatedProducts(id, 9);
            model.UpsellProducts = _productService.GetUpsellProducts(6);
            model.ProductImages = _productService.GetImages(id);
            model.Tags = _productService.GetProductTags(id);
            //model.Colors = _orderService.GetColors().Select(x => new SelectListItem()
            //{
            //    Text = x.Name,
            //    Value = x.Id.ToString()
            //}).ToList();
            model.Colors = _productService.GetAvailableColor(id).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();



            //if (model.Category.Id == 1022 || model.Category.Id == 1021)
            //    model.Sizes = _orderService.GetSizes(3).Select(x => new SelectListItem()
            //    {
            //        Text = x.Name,
            //        Value = x.Id.ToString()
            //    }).ToList();
            //else if (model.Category.Id == 1014 || model.Category.Id == 1023 || model.Category.Id == 1025 || model.Category.Id == 1027)
            //    model.Sizes = _orderService.GetSizes(1).Select(x => new SelectListItem()
            //    {
            //        Text = x.Name,
            //        Value = x.Id.ToString()
            //    }).ToList();
            //else
            //    model.Sizes = _orderService.GetSizes(2).Select(x => new SelectListItem()
            //    {
            //        Text = x.Name,
            //        Value = x.Id.ToString()
            //    }).ToList();
            model.Sizes = _productService.GetAvailableSize(id).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            return View(model);
        }
    }
}