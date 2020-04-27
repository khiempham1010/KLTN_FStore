using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AtomStore.Controllers
{
    [Route("about.html")]
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            return View();
        }
    }
}