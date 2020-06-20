using AtomStore.Aplication.Dapper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtomStore.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public List<RevenueReportViewModel> RevenueReports { get; set; }
        public int TotalUser { get; set; }
        public int TotalRevenue { get; set; }
        public int TotalProfit { get; set; }
        public int Expenditure { get; set; }
    }
}
