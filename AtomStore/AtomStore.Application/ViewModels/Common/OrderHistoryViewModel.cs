using AtomStore.Application.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.ViewModels.Common
{
    public class OrderHistoryViewModel
    {
        public OrderViewModel Order { get; set; }
        public ICollection<OrderDetailViewModel> OrderDetail { get; set; }
        public ICollection<ProductViewModel> Product { get; set; }
        public decimal Total { get; set; }
    }
}
