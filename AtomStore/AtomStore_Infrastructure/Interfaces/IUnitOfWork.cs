using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Call save change from DBContext
        /// </summary>
        void Commit();
    }
}
