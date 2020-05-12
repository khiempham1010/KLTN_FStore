using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AtomStore.Controllers
{
    public class WishlistController : Controller
    {
        [HttpGet]
        [Route("wishlist.html")]
        public IActionResult Wishlist()
        {
            return View();
        }
    }
}