using AtomStore.Application.ViewModels.Common;
using AtomStore.Application.ViewModels.Product;
using AtomStore.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomStore.Utilities.Extensions;
using AtomStore.Application.ViewModels.System;

namespace AtomStore.Models
{
    public class CheckoutViewModel: OrderViewModel
    {
        public List<ShoppingCartViewModel> Carts { get; set; }
        public AppUserViewModel AppUserViewModel { get; set; }
        public List<EnumModel> PaymentMethods
        {
            get
            {
                return ((PaymentMethod[])Enum.GetValues(typeof(PaymentMethod)))
                    .Select(c => new EnumModel
                    {
                        Value = (int)c,
                        Name = c.GetDescription()
                    }).ToList();
            }
        }
    }
}
