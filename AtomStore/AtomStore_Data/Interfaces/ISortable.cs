using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.Interfaces
{
    public interface ISortable
    {
        int SortOrder { get; set; }
    }
}
