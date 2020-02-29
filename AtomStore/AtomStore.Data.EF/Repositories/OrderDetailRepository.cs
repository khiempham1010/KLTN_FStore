using AtomStore.Data.Entities;
using AtomStore.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.EF.Repositories
{
    public class OrderDetailRepository : EFRepository<OrderDetail, int>, IOrderDetailRepository
    {
        public OrderDetailRepository(AppDbContext context) : base(context)
        {
        }
    }
}
