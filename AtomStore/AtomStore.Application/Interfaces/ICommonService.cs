using AtomStore.Application.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Application.Interfaces
{
    public interface ICommonService
    {
        SystemConfigViewModel GetSystemConfig(string code);
    }
}
