using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AtomStore.Data.Entities
{
    public class OrderDetail:DomainEntity<int>
    {
        public OrderDetail() { }
        public OrderDetail(int id, int orderId, int productId, int quantity, decimal price, int colorId, int sizeId)
        {
            Id = id;
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            ColorId = colorId;
            SizeId = sizeId;
        }

        public OrderDetail(int orderId, int productId, int quantity, decimal price, int colorId, int sizeId)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            ColorId = colorId;
            SizeId = sizeId;
        }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("ColorId")]
        public virtual Color Color { set; get; }
        [ForeignKey("SizeId")]
        public virtual Size Size { set; get; }
    }
}
