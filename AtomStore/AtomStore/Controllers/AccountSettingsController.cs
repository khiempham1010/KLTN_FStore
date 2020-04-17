using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Interfaces;
using AtomStore.Extensions;
using AtomStore.Models;
using AtomStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AtomStore.Controllers
{
    public class AccountSettingsController : Controller
    {
        IViewRenderService _viewRenderService;
        IConfiguration _configuration;
        IEmailSender _emailSender;
        IUserService _userService;
        public AccountSettingsController( IViewRenderService viewRenderService, IEmailSender emailSender,
            IConfiguration configuration, IUserService userService)
        {
            _viewRenderService = viewRenderService;
            _configuration = configuration;
            _emailSender = emailSender;
            _userService = userService;
        }
        [HttpGet]
        [Route("accountsettings.html", Name = "AccountSettings")]
        public IActionResult AccountSettings()
        {
            var model = new AccountSettingsViewModel();
           
            if (User.Identity.IsAuthenticated == true)
            {
                model.AppUserViewModel = _userService.GetById(User.GetSpecificClaim("UserId").ToString()).Result;

            }
            return View(model);
        }

        [HttpPost]
        [Route("accountsettings.html", Name = "AccountSettings")]
        public IActionResult AccountSettings(AccountSettingsViewModel model)
        {

            return View(model);
        }
    }
}