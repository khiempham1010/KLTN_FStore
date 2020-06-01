using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Application.ViewModels.System;
using AtomStore.Data.Enums;
using AtomStore.Extensions;
using AtomStore.Models;
using AtomStore.Services;
using AtomStore.Utilities.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace AtomStore.Controllers
{
    public class CartController : Controller
    {
        IProductService _productService;
        IOrderService _orderService;
        IViewRenderService _viewRenderService;
        IConfiguration _configuration;
        IEmailSender _emailSender;
        IUserService _userService;
        public CartController(IProductService productService,
            IViewRenderService viewRenderService, IEmailSender emailSender,
            IConfiguration configuration, IOrderService orderService,
            IUserService userService)
        {
            _productService = productService;
            _orderService = orderService;
            _viewRenderService = viewRenderService;
            _configuration = configuration;
            _emailSender = emailSender;
            _userService = userService;
        }
        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("checkout.html", Name = "Checkout")]
        [HttpGet]
        public IActionResult Checkout()
        {
            var model = new CheckoutViewModel();
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);
            
            if (session.Any(x => x.Color == null || x.Size == null))
            {
                return Redirect("/cart.html");
            }

            model.Carts = session;
            if (User.Identity.IsAuthenticated == true)
            {
                model.AppUserViewModel = _userService.GetById(User.GetSpecificClaim("UserId").ToString()).Result;
            }
            return View(model);
        }

        [Route("checkout.html", Name = "Checkout")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel model)
        {

            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);

            if (ModelState.IsValid)
            {
                if (session != null)
                {
                    var details = new List<OrderDetailViewModel>();
                    foreach (var item in session)
                    {
                        details.Add(new OrderDetailViewModel()
                        {
                            Product = item.Product,
                            Price = item.Price,
                            ColorId = item.Color.Id,
                            SizeId = item.Size.Id,
                            Quantity = item.Quantity,
                            ProductId = item.Product.Id
                        });
                    }
                    var orderViewModel = new OrderViewModel()
                    {
                        CustomerPhone = model.CustomerPhone,
                        OrderStatus = OrderStatus.New,
                        CustomerAddress = model.CustomerAddress,
                        CustomerEmail = model.CustomerEmail,
                        CustomerName = model.CustomerName,
                        CustomerMessage = model.CustomerMessage,
                        OrderDetails = details,
                        PaymentMethod = model.PaymentMethod,
                        DateCreated = DateTime.Now
                    };
                    if (User.Identity.IsAuthenticated == true)
                    {
                        orderViewModel.CustomerId = Guid.Parse(User.GetSpecificClaim("UserId"));
                    }

                    _orderService.Create(orderViewModel);
                    try
                    {
                        _orderService.Save();
                        HttpContext.Session.Remove(CommonConstants.CartSession);
                        ViewData["Success"] = true;
                    }
                    catch (Exception ex)
                    {
                        ViewData["Success"] = false;
                        ModelState.AddModelError("", ex.Message);
                    }

                }
            }
            model.Carts = session;
            return View();
        }

        [Route("checkoutPaypal.html", Name = "CheckoutPaypal")]
        [HttpGet]
        public IActionResult CheckoutWithPaypal()
        {
            var model = new CheckoutViewModel();
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);

            if (session.Any(x => x.Color == null || x.Size == null))
            {
                return Redirect("/cart.html");
            }

            model.Carts = session;
            if (User.Identity.IsAuthenticated == true)
            {
                model.AppUserViewModel = _userService.GetById(User.GetSpecificClaim("UserId").ToString()).Result;
            }
            return View(model);
        }

        [Route("checkoutPaypal.html", Name = "CheckoutPaypal")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult CheckoutWithPayPal(CheckoutViewModel model)
        {
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);

            if (ModelState.IsValid)
            {
                if (session != null)
                {
                    var details = new List<OrderDetailViewModel>();
                    foreach (var item in session)
                    {
                        details.Add(new OrderDetailViewModel()
                        {
                            Product = item.Product,
                            Price = item.Price,
                            ColorId = item.Color.Id,
                            SizeId = item.Size.Id,
                            Quantity = item.Quantity,
                            ProductId = item.Product.Id
                        });
                    }
                    var orderViewModel = new OrderViewModel()
                    {
                        CustomerPhone = model.CustomerPhone,
                        OrderStatus = OrderStatus.New,
                        CustomerAddress = model.CustomerAddress,
                        CustomerEmail = model.CustomerEmail,
                        CustomerName = model.CustomerName,
                        CustomerMessage = model.CustomerMessage,
                        OrderDetails = details,
                        PaymentMethod = model.PaymentMethod,
                        DateCreated = DateTime.Now
                    };
                    if (User.Identity.IsAuthenticated == true)
                    {
                        orderViewModel.CustomerId = Guid.Parse(User.GetSpecificClaim("UserId"));
                    }

                    _orderService.Create(orderViewModel);
                    try
                    {
                        _orderService.Save();
                        HttpContext.Session.Remove(CommonConstants.CartSession);
                        ViewData["Success"] = true;
                    }
                    catch (Exception ex)
                    {
                        ViewData["Success"] = false;
                        ModelState.AddModelError("", ex.Message);
                    }

                }
            }
            model.Carts = session;
            return View();
        }

        public IActionResult CheckoutWithStripe(CheckoutViewModel model, string stripeEmail, string stripeToken)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();
            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = 500,
                Description = "Test Payment",
                Currency = "usd",
                CustomerId = customer.Id,
                ReceiptEmail = stripeEmail,
                Metadata = new Dictionary<string, string>()
                {
                    {"OrderId","111" },
                    {"Postcode","LEE111" },
                }
            });

            if (charge.Status == "Succeeded")
            {
                string BalanceTransactionId = charge.BalanceTransactionId;
                return View();
            }
            else
            {

            }
            return View();
        }

        #region AJAX Request

        /// <summary>
        /// Get list item
        /// </summary>
        /// <returns></returns>
        public IActionResult GetCart()
        {
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);

            if (session == null)
            {
                session = new List<ShoppingCartViewModel>();
                
            }

            foreach(var item in session)
            {
                item.Colors = _productService.GetAvailableColor(item.Product.Id);
                item.Sizes = _productService.GetAvailableSize(item.Product.Id);
            }

            return new OkObjectResult(session);
        }

        /// <summary>
        /// Remove all products in cart
        /// </summary>
        /// <returns></returns>
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CommonConstants.CartSession);
            return new OkObjectResult("OK");
        }

        /// <summary>
        /// Add product to cart
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, int color, int size)
        {
            //Get product detail
            var product = _productService.GetById(productId);

            //Get session with item list from cart
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);
            if (session != null)
            {
                //Convert string to list object
                bool hasChanged = false;

                //Check exist with item product id
                if (session.Any(x => x.Product.Id == productId))
                {
                    foreach (var item in session)
                    {
                        //Update quantity for product if match product id
                        if (item.Product.Id == productId)
                        {
                            item.Quantity += quantity;
                            item.Price = product.PromotionPrice ?? product.Price;
                            hasChanged = true;
                        }
                    }
                }
                else
                {
                    session.Add(new ShoppingCartViewModel()
                    {
                        Product = product,
                        Quantity = quantity,
                        Color = _orderService.GetColor(color),
                        Size = _orderService.GetSize(size),
                        Price = product.PromotionPrice ?? product.Price
                    });
                    hasChanged = true;
                }

                //Update back to cart
                if (hasChanged)
                {
                    HttpContext.Session.Set(CommonConstants.CartSession, session);
                }
            }
            else
            {
                //Add new cart
                var cart = new List<ShoppingCartViewModel>();
                cart.Add(new ShoppingCartViewModel()
                {
                    Product = product,
                    Quantity = quantity,
                    Color = _orderService.GetColor(color),
                    Size = _orderService.GetSize(size),
                    Price = product.PromotionPrice ?? product.Price
                });
                HttpContext.Session.Set(CommonConstants.CartSession, cart);
                
                var a=HttpContext.Session.GetString(CommonConstants.CartSession);
            }
            return new OkObjectResult(productId);
        }

        /// <summary>
        /// Remove a product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IActionResult RemoveFromCart(int productId)
        {
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);
            if (session != null)
            {
                bool hasChanged = false;
                foreach (var item in session)
                {
                    if (item.Product.Id == productId)
                    {
                        session.Remove(item);
                        hasChanged = true;
                        break;
                    }
                }
                if (hasChanged)
                {
                    HttpContext.Session.Set(CommonConstants.CartSession, session);
                }
                return new OkObjectResult(productId);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Update product quantity
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public IActionResult UpdateCart(int productId, int quantity, int color, int size)
        {
            var session = HttpContext.Session.Get<List<ShoppingCartViewModel>>(CommonConstants.CartSession);
            if (session != null)
            {
                bool hasChanged = false;
                foreach (var item in session)
                {
                    if (item.Product.Id == productId)
                    {
                        if (quantity > 0 && color > 0 && size > 0)
                        {
                            if (_productService.CheckAvailability(productId, size, color, quantity))
                            {
                                var product = _productService.GetById(productId);
                                item.Product = product;
                                item.Size = _orderService.GetSize(size);
                                item.Color = _orderService.GetColor(color);
                                item.Quantity = quantity;
                                item.Price = product.PromotionPrice ?? product.Price;
                                hasChanged = true;
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                        else
                        {
                            var product = _productService.GetById(productId);
                            item.Product = product;
                            item.Size = _orderService.GetSize(size);
                            item.Color = _orderService.GetColor(color);
                            item.Quantity = quantity;
                            item.Price = product.PromotionPrice ?? product.Price;
                            hasChanged = true;
                        }
                    }
                }
                if (hasChanged)
                {                   
                    HttpContext.Session.Set(CommonConstants.CartSession, session);
                }
                return new OkObjectResult(productId);
            }
            return new EmptyResult();
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

        #endregion

    }
}