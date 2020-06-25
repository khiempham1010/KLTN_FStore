using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Interfaces
{
    public interface IVisitorCounterService
    {
        int GetVisitors();
        void SetVisitors();
    }
}
