using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.ViewModels.Product
{
    public class WishlistViewModel
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public ProductViewModel Product { set; get; }
    }
}
