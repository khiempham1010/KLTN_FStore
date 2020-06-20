using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Aplication.Dapper.Interfaces;
using AtomStore.Application.Interfaces;
using AtomStore.Areas.Admin.Models;
using AtomStore.Data.EF.Repositories;
using AtomStore.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AtomStore.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        IReportService _reportService;
        IUserService _userService;
        IOrderService _orderService;
        public HomeController(IReportService reportService,IUserService userService, IOrderService orderService)
        {
            _reportService = reportService;
            _userService = userService;
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            var email = User.GetSpecificClaim("Email");
            return View();
        }
        public  IActionResult GetRevenue(string fromDate, string toDate)
        {
            var dashBoardVM = new DashboardViewModel();
            dashBoardVM.RevenueReports = _reportService.GetReportAsync(fromDate, toDate).Result.ToList();
            dashBoardVM.TotalUser = _userService.GetUserCount();
            dashBoardVM.TotalRevenue = _orderService.GetTotalRevenue();
            dashBoardVM.TotalProfit = _orderService.GetTotalProfit();
            return new OkObjectResult(dashBoardVM);
        }
    }
}