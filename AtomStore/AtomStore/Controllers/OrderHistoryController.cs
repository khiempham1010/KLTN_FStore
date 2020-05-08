using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Interfaces;
using AtomStore.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AtomStore.Controllers
{
    public class OrderHistoryController : Controller
    {
        IOrderService _orderService;
        UserManager<AppUser> _userManager;
        public OrderHistoryController(IOrderService orderService, UserManager<AppUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("history.html")]
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            var orderHistory = _orderService.GetOrderHistory(user.Email);
            foreach (var item in orderHistory)
            {
                decimal total = 0;
                foreach(var subItem in item.OrderDetail)
                {
                    total += subItem.Price * subItem.Quantity;
                }
                item.Total = total;
            }
            return View(orderHistory);
        }
    }
}