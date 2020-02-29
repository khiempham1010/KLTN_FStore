using AtomStore.Data.Entities;
using AtomStore.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.EF.Repositories
{
    public class SizeRepository : EFRepository<Size, int>, ISizeRepository
    {
        public SizeRepository(AppDbContext context) : base(context)
        {
        }
    }
}
