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
        IProductFeedbackService _feedbackService;
        IProductService _productService;
        IVisitorCounterService _visitorCounterService;
        public HomeController(IReportService reportService,
            IUserService userService, 
            IOrderService orderService,
            IProductFeedbackService feedbackService,
            IProductService productService,
            IVisitorCounterService visitorCounterService)
        {
            _reportService = reportService;
            _userService = userService;
            _orderService = orderService;
            _feedbackService = feedbackService;
            _productService = productService;
            _visitorCounterService = visitorCounterService;
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
            dashBoardVM.Expense = dashBoardVM.TotalRevenue - dashBoardVM.TotalProfit;
            dashBoardVM.Expenditure = 0;
            dashBoardVM.Sales = _orderService.GetTotalSales();
            var a = _productService.GetInstockProduct();
            dashBoardVM.SalePercent = dashBoardVM.Sales*100 / (dashBoardVM.Sales + a);
            dashBoardVM.Review = _feedbackService.GetAll().Count();
            dashBoardVM.ReviewPercent = dashBoardVM.Review*100 / dashBoardVM.Sales;
            dashBoardVM.Visittor = _visitorCounterService.GetVisitors();
            return new OkObjectResult(dashBoardVM);
        }
    }
}