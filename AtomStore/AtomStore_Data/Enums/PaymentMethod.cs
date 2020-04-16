using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AtomStore.Data.Enums
{
    public enum PaymentMethod
    {
        [Description("Cash On Delivery")]
        CashOnDelivery,
        [Description("Visa")]
        Visa,
        [Description("Master Card")]
        MasterCard,
        [Description("PayPal")]
        PayPal,
        [Description("ATM")]
        Atm
    }
}
