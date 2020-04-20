using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.System;
using AtomStore.Data.Entities;
using AtomStore.Extensions;
using AtomStore.Models;
using AtomStore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomStore.Controllers
{
    public class AccountSettingsController : Controller
    {
        IViewRenderService _viewRenderService;
        IConfiguration _configuration;
        IEmailSender _emailSender;
        IUserService _userService;
        UserManager<AppUser> _userManager;
        public AccountSettingsController(IViewRenderService viewRenderService, IEmailSender emailSender,
            IConfiguration configuration, IUserService userService, UserManager<AppUser> userManager)
        {
            _viewRenderService = viewRenderService;
            _configuration = configuration;
            _emailSender = emailSender;
            _userService = userService;
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { get; set; }
        [HttpGet]
        [Route("accountsettings.html", Name = "AccountSettings")]
        public async Task<IActionResult> AccountSettings()
        {
            //var model = new AccountSettingsViewModel();

            //if (User.Identity.IsAuthenticated == true)
            //{
            //    model.AppUserViewModel = _userService.GetById(User.GetSpecificClaim("UserId").ToString()).Result;

            //}
            //return View(model);          

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var model = new AccountSettingsViewModel
            {
                FullName = user.FullName,
                BirthDay = user.BirthDay,
                Address = user.Address,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                StatusMessage = StatusMessage
            };

            return View(model);
            //AppUserViewModel user = await _userService.GetById(User.GetSpecificClaim("UserId").ToString());
            //if (user == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}

            //return View(user);
        }

        [HttpPost]
        [Route("accountsettings.html", Name = "AccountSettings")]
        public async Task<IActionResult> AccountSettings(AppUserViewModel userVm)
        {

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                if (userVm.Email != null)
                {
                    await _userService.UpdateUserAsync(userVm);
                    StatusMessage = "Your account information has been updated.";
                }

            }

            return RedirectToAction(nameof(AccountSettings));

            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}
            //if (model.Email != null)
            //{
            //    await _userManager.UpdateAsync(user);
            //}

            //StatusMessage = "Your profile has been updated";
            //return RedirectToAction(nameof(AccountSettings));
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }


        #endregion
    }
}