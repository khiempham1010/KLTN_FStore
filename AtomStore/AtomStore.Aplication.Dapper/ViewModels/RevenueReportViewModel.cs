using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Aplication.Dapper.ViewModels
{
    public class RevenueReportViewModel
    {
        public DateTime Date { get; set; }
        public Decimal Revenue { get; set; }
        public Decimal Profit { get; set; }
    }
}
