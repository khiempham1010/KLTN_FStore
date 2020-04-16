using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace AtomStore.Controllers
{
    public class AccountSettingsController : Controller
    {
        [HttpGet]
        [Route("accountsettings.html", Name = "AccountSettings")]
        public IActionResult AccountSettings()
        {
            return View();
        }

        [HttpPost]
        [Route("accountsettings.html", Name = "AccountSettings")]
        public async Task<IActionResult> AccountSettings(AccountSettingsViewModel model)
        {
            return View();
        }
    }
}