using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AtomStore.Controllers
{
    public class ViewedlistController : Controller
    {
        private readonly IViewedlistService _viewedlistService;
        public readonly UserManager<AppUser> _userManager;
        private readonly IWishlistService _wishlistService;
        public ViewedlistController(IViewedlistService viewdlistService, UserManager<AppUser> userManager,IWishlistService wishlistService)
        {
            _viewedlistService = viewdlistService;
            _userManager = userManager;
            _wishlistService = wishlistService;
        }
        [HttpGet]
        [Route("viewedlist.html", Name = "Viewed list")]
        public IActionResult Viewedlist()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetViewedlist()
        {
            var curUser = _userManager.GetUserAsync(User).Result;
            List<ViewedlistViewModel> viewedlistVMList = _viewedlistService.GetAll(curUser.Id);
            return new OkObjectResult(viewedlistVMList);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllPaging(int page)
        {
            var curUser = _userManager.GetUserAsync(User).Result;
            var model = _viewedlistService.GetAllPaging(curUser.Id, page, 12);
            foreach (var item in model.Results)
            {
                item.Product.Wishlist = _wishlistService.GetByProductAndUserId(item.ProductId, curUser.Id) == default ? false : true;
            }
            return new OkObjectResult(model);
        }
    }
}