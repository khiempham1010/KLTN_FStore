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
        public ViewedlistController(IViewedlistService viewdlistService, UserManager<AppUser> userManager)
        {
            _viewedlistService = viewdlistService;
            _userManager = userManager;
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
            return new OkObjectResult(model);
        }
    }
}