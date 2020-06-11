using AtomStore.Application.Interfaces;
using AtomStore.Application.ViewModels.Common;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Entities;
using AtomStore.Data.Enums;
using AtomStore.Infrastructure.Interfaces;
using AtomStore.Utilities.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AtomStore.Application.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<OrderDetail, int> _orderDetailRepository;
        private readonly IRepository<Color, int> _colorRepository;
        private readonly IRepository<Size, int> _sizeRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<SizeType, int> _sizeTypeRepository;
        private readonly IUnitOfWork _unitOfWork;


        public OrderService(IRepository<Order, int> orderRepository,
            IRepository<OrderDetail, int> orderDetailRepository,
            IRepository<Color, int> colorRepository,
            IRepository<Product, int> productRepository,
            IRepository<Size, int> sizeRepository,
            IRepository<SizeType, int> sizeTypeRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _colorRepository = colorRepository;
            _sizeRepository = sizeRepository;
            _sizeTypeRepository = sizeTypeRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public void Create(OrderViewModel OrderVm)
        {
            var order = Mapper.Map<OrderViewModel, Order>(OrderVm);
            var orderDetails = Mapper.Map<ICollection<OrderDetailViewModel>, ICollection<OrderDetail>>(OrderVm.OrderDetails);
            foreach (var detail in orderDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
            }
            order.OrderDetails = orderDetails;
            _orderRepository.Add(order);
        }

        public void Update(OrderViewModel OrderVm)
        {
            //Mapping to order domain
            var order = Mapper.Map<OrderViewModel, Order>(OrderVm);

            //Get order Detail
            var newDetails = order.OrderDetails;

            //new details added
            var addedDetails = newDetails.Where(x => x.Id == 0).ToList();

            //get updated details
            var updatedDetails = newDetails.Where(x => x.Id != 0).ToList();

            //Existed details
            var existedDetails = _orderDetailRepository.FindAll(x => x.OrderId == OrderVm.Id);

            //Clear db
            order.OrderDetails.Clear();

            foreach (var detail in updatedDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
                _orderDetailRepository.Update(detail);
            }

            foreach (var detail in addedDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
                _orderDetailRepository.Add(detail);
            }

            _orderDetailRepository.RemoveMultiple(existedDetails.Except(updatedDetails).ToList());

            _orderRepository.Update(order);
        }

        public void UpdateStatus(int OrderId, OrderStatus status)
        {
            var order = _orderRepository.FindById(OrderId);
            order.OrderStatus = status;
            _orderRepository.Update(order);
        }

        public List<SizeViewModel> GetSizes()
        {
            return _sizeRepository.FindAll().OrderBy(x=>x.SizeType).ProjectTo<SizeViewModel>().ToList();
        }

        public List<SizeViewModel> GetSizes(int sizeType)
        {
            return _sizeRepository.FindAll(x => x.SizeType.Id == sizeType).ProjectTo<SizeViewModel>().ToList();
        }

        public OrderViewModel GetById(int id)
        {
            return Mapper.Map<Order,OrderViewModel>( _orderRepository.FindById(id));
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public PagedResult<OrderViewModel> GetAllPaging(string startDate, string endDate, string keyword
            , int pageIndex, int pageSize)
        {
            var query = _orderRepository.FindAll();
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime start = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.DateCreated >= start);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime end = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.DateCreated <= end);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.CustomerName.Contains(keyword) || x.CustomerPhone.Contains(keyword));
            }
            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.DateCreated)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<OrderViewModel>()
                .ToList();
            return new PagedResult<OrderViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public OrderViewModel GetDetail(int OrderId)
        {
            var Order = _orderRepository.FindSingle(x => x.Id == OrderId);
            var OrderVm = Mapper.Map<Order, OrderViewModel>(Order);
            var OrderDetailVm = _orderDetailRepository.FindAll(x => x.OrderId == OrderId).ProjectTo<OrderDetailViewModel>().ToList();
            OrderVm.OrderDetails = OrderDetailVm;
            return OrderVm;
        }

        public List<OrderDetailViewModel> GetOrderDetails(int OrderId)
        {
            return _orderDetailRepository
                .FindAll(x => x.OrderId == OrderId, c => c.Order, c => c.Color, c => c.Size, c => c.Product)
                .ProjectTo<OrderDetailViewModel>().ToList();
        }

        public List<ColorViewModel> GetColors()
        {
            return _colorRepository.FindAll().ProjectTo<ColorViewModel>().ToList();
        }

        public OrderDetailViewModel CreateDetail(OrderDetailViewModel OrderDetailVm)
        {
            var OrderDetail = Mapper.Map<OrderDetailViewModel, OrderDetail>(OrderDetailVm);
            _orderDetailRepository.Add(OrderDetail);
            return OrderDetailVm;
        }

        public void DeleteDetail(int productId, int OrderId, int colorId, int sizeId)
        {
            var detail = _orderDetailRepository.FindSingle(x => x.ProductId == productId
           && x.OrderId == OrderId && x.ColorId == colorId && x.SizeId == sizeId);
            _orderDetailRepository.Remove(detail);
        }

        public ColorViewModel GetColor(int id)
        {
            return Mapper.Map<Color, ColorViewModel>(_colorRepository.FindById(id));
        }

        public SizeViewModel GetSize(int id)
        {
            return Mapper.Map<Size, SizeViewModel>(_sizeRepository.FindById(id));
        }

        public List<OrderHistoryViewModel> GetOrderHistory(String userEmail)
        {
            var listUserOrder = _orderRepository.FindAll(x => x.CustomerEmail == userEmail).OrderByDescending(x=>x.DateCreated);
            List<OrderHistoryViewModel> orderHistory = new List<OrderHistoryViewModel>();
            foreach (var item in listUserOrder)
            {
                orderHistory.Add(GetOneOrderHistory(item.Id));
            }
            return orderHistory;
        }

        public OrderHistoryViewModel GetOneOrderHistory(int orderId)
        {
            OrderHistoryViewModel orderHistory = new OrderHistoryViewModel();
            orderHistory.Order = Mapper.Map<Order, OrderViewModel>(_orderRepository.FindById(orderId));
            orderHistory.OrderDetail = _orderDetailRepository.FindAll(x => x.OrderId == orderHistory.Order.Id).ProjectTo<OrderDetailViewModel>().ToList();
            orderHistory.Product = new List<ProductViewModel>();
            foreach (var item in orderHistory.OrderDetail)
            {
                var a = _productRepository.FindById(item.ProductId);
                orderHistory.Product.Add(Mapper.Map<Product, ProductViewModel>(_productRepository.FindById(item.ProductId)));
            }
            return orderHistory;
        }
    }
}
