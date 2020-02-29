using AtomStore.Data.Entities;
using AtomStore.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.IRepositories
{
    public interface IPermissionRepository : IRepository<Permission, int>
    {
    }
}
