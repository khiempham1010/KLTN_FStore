using AtomStore.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.Interfaces
{
    public interface ISwitchable
    {
        Status Status { get; set; }
    }
}
