using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Common;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Enums;
using AtomStore.Services;
using AtomStore.Utilities.Extensions;
using AtomStore.Utilities.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OfficeOpenXml;

namespace AtomStore.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IViewRenderService _viewRenderService;
        private readonly IEmailSender _emailSender;
        public OrderController(
            IOrderService orderService, 
            IHostingEnvironment hostingEnvironment, 
            IViewRenderService viewRenderService,
            IEmailSender emailSender )
        {
            _orderService = orderService;
            _hostingEnvironment = hostingEnvironment;
            _viewRenderService = viewRenderService;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _orderService.GetDetail(id);

            return new OkObjectResult(model);
        }
        [HttpGet]
        public IActionResult UpdateStatus(int orderId, OrderStatus status)
        {
            _orderService.UpdateStatus(orderId, status);

            return new OkResult();
        }
        [HttpGet]
        public IActionResult GetAllPaging(string startDate, string endDate, string keyword, int page, int pageSize)
        {
            var model = _orderService.GetAllPaging(startDate, endDate, keyword, page, pageSize);
            return new OkObjectResult(model);
        }
        [HttpPost]
        public IActionResult SaveEntity(OrderViewModel orderVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            if (orderVm.Id == 0)
            {
                _orderService.Create(orderVm);
            }
            else
            {
                _orderService.Update(orderVm);
            }
            _orderService.Save();
            return new OkObjectResult(orderVm);
        }
        [HttpPost]
        public async Task<IActionResult> SendMail(OrderViewModel orderVm)
        {
            var order = _orderService.GetDetail(orderVm.Id);
            orderVm.CustomerEmail = order.CustomerEmail;
            orderVm.DateCreated = order.DateCreated;
            orderVm.DateModified = order.DateModified;
            if (order.CustomerId != null)
            {
                orderVm.CustomerId = order.CustomerId;
            }
            orderVm.OrderDetails = order.OrderDetails;
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            if (orderVm.Id == 0)
            {
                
            }
            else
            {
                try
                {
                    //Send mail
                    //await _emailSender.SendEmailAsync(_configuration["MailSettings:AdminMail"], "New bill from ATOM Store", content);
                    if (orderVm.CustomerEmail != null)
                    {

                        var content = await _viewRenderService.RenderToStringAsync("Cart/_BillMail", orderVm);
                        //Send mail
                        await _emailSender.SendEmailAsync(orderVm.CustomerEmail.ToString(), "New order from Atom Store", content);
                    }
                    foreach(var item in orderVm.OrderDetails)
                    {
                        _orderService.DecreaseQuantity(item.ProductId, item.SizeId, item.ColorId, item.Quantity);
                    }
                    _orderService.UpdateStatus(orderVm.Id,orderVm.OrderStatus);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            _orderService.Save();
            return new OkObjectResult(orderVm);
        }

        [HttpPost]
        public IActionResult ChangeOrderStatus(OrderViewModel orderVm)
        {
            _orderService.UpdateStatus(orderVm.Id , orderVm.OrderStatus);
            _orderService.Save();
            return new OkObjectResult(orderVm);
        }

        [HttpGet]
        public IActionResult GetPaymentMethod()
        {
            List<EnumModel> enums = ((PaymentMethod[])Enum.GetValues(typeof(PaymentMethod)))
                .Select(c => new EnumModel()
                {
                    Value = (int)c,
                    Name = c.GetDescription()
                }).ToList();
            return new OkObjectResult(enums);
        }

        [HttpGet]
        public IActionResult GetOrderStatus()
        {
            List<EnumModel> enums = ((OrderStatus[])Enum.GetValues(typeof(OrderStatus)))
                .Select(c => new EnumModel()
                {
                    Value = (int)c,
                    Name = c.GetDescription()
                }).ToList();
            return new OkObjectResult(enums);
        }

        [HttpGet]
        public IActionResult GetColors()
        {
            var colors = _orderService.GetColors();
            return new OkObjectResult(colors);
        }

        [HttpGet]
        public IActionResult GetSizes()
        {
            var sizes = _orderService.GetSizes();
            return new OkObjectResult(sizes);
        }
        [HttpPost]
        public IActionResult ExportExcel(int orderId)
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = $"Order_{orderId}.xlsx";
            // Template File
            string templateDocument = Path.Combine(sWebRootFolder, "templates", "OrderTemplate.xlsx");

            string url = $"{Request.Scheme}://{Request.Host}/{"export-files"}/{sFileName}";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, "export-files", sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(templateDocument))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["TEDUOrder"];
                    // Data Acces, load order header data.
                    var orderDetail = _orderService.GetDetail(orderId);

                    // Insert customer data into template
                    worksheet.Cells[4, 1].Value = "Customer Name: " + orderDetail.CustomerName;
                    worksheet.Cells[5, 1].Value = "Address: " + orderDetail.CustomerAddress;
                    worksheet.Cells[6, 1].Value = "Phone: " + orderDetail.CustomerPhone;
                    // Start Row for Detail Rows
                    int rowIndex = 9;

                    // load order details
                    var orderDetails = _orderService.GetOrderDetails(orderId);
                    int count = 1;
                    foreach (var item in orderDetails)
                    {
                        // Cell 1, Carton Count
                        worksheet.Cells[rowIndex, 1].Value = count.ToString();
                        // Cell 2, Order Number (Outline around columns 2-7 make it look like 1 column)
                        worksheet.Cells[rowIndex, 2].Value = item.Product.Name;
                        // Cell 8, Weight in LBS (convert KG to LBS, and rounding to whole number)
                        worksheet.Cells[rowIndex, 3].Value = item.Quantity.ToString();

                        worksheet.Cells[rowIndex, 4].Value = item.Price.ToString("N0");
                        worksheet.Cells[rowIndex, 5].Value = (item.Price * item.Quantity).ToString("N0");
                        // Increment Row Counter
                        rowIndex++;
                        count++;
                    }
                    decimal total = (decimal)(orderDetails.Sum(x => x.Quantity * x.Price));
                    worksheet.Cells[24, 5].Value = total.ToString("N0");

                    var numberWord = "Total amount (by word): " + TextHelper.ToString(total);
                    worksheet.Cells[26, 1].Value = numberWord;
                    var orderDate = orderDetail.DateCreated;
                    worksheet.Cells[28, 3].Value = orderDate.Day + ", " + orderDate.Month + ", " + orderDate.Year;


                    package.SaveAs(file); //Save the workbook.
                }
            }
            return new OkObjectResult(url);
        }
    }
}