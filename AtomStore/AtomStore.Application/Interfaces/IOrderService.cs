using AtomStore.Application.ViewModels.Common;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Enums;
using AtomStore.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Interfaces
{
    public interface IOrderService
    {
        void Create(OrderViewModel OrderVm);
        void Update(OrderViewModel OrderVm);

        PagedResult<OrderViewModel> GetAllPaging(string startDate, string endDate, string keyword,
            int pageIndex, int pageSize);

        OrderViewModel GetDetail(int OrderId);

        OrderDetailViewModel CreateDetail(OrderDetailViewModel OrderDetailVm);

        void DeleteDetail(int productId, int OrderId, int colorId, int sizeId);

        void UpdateStatus(int orderId, OrderStatus status);

        List<OrderDetailViewModel> GetOrderDetails(int OrderId);

        List<ColorViewModel> GetColors();

        List<SizeViewModel> GetSizes();
        List<SizeViewModel> GetSizes(int sizeType);

        ColorViewModel GetColor(int id);

        SizeViewModel GetSize(int id);

        List<OrderHistoryViewModel> GetOrderHistory(String userEmail);

        OrderHistoryViewModel GetOneOrderHistory(int orderId);

        void Save();
    }
}
