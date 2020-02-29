using AtomStore.Data.Entities;
using AtomStore.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.EF.Repositories
{
    public class TagRepository : EFRepository<Tag, string>, ITagRepository
    {
        public TagRepository(AppDbContext context) : base(context)
        {
        }
    }
}
