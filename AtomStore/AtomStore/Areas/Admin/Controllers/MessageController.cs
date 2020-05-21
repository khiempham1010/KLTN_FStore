using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AtomStore.Areas.Admin.Controllers
{
    public class MessageController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}