using AtomStore.Data.Entities;
using AtomStore.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.EF.Repositories
{
    public class ProductRepository : EFRepository<Product,int>,IProductRepository
    {
        public ProductRepository(AppDbContext context):base(context)
        {
             
        }
    }
}
