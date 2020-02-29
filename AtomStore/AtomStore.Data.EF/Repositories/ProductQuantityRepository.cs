using AtomStore.Data.Entities;
using AtomStore.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.EF.Repositories
{
    public class ProductQuantityRepository : EFRepository<ProductQuantity, int>, IProductQuantityRepository
    {
        public ProductQuantityRepository(AppDbContext context) : base(context)
        {
        }
    }
}
