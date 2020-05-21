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
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;
        public readonly UserManager<AppUser> _userManager;
        public WishlistController(IWishlistService wishlistService, UserManager<AppUser> userManager)
        {
            _wishlistService = wishlistService;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("wishlist.html")]
        public IActionResult Wishlist()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetWishlist()
        {
            var curUser = _userManager.GetUserAsync(User).Result;
            List<WishlistViewModel> wishlistVMList = _wishlistService.GetAll(curUser.Id);
            return new OkObjectResult(wishlistVMList);
        }

        [HttpPost]
        public IActionResult Remove(int wishlistId)
        {
            _wishlistService.Delete(wishlistId);
            _wishlistService.Save();
            return new OkObjectResult(wishlistId);
        }
    }
}