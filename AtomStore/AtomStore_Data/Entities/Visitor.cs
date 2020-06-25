using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomStore.Data.Entities
{
    public class Visitor:DomainEntity<int>
    {

        public Visitor(int numberOfVisitors)
        {
            NumberOfVisitors = numberOfVisitors;
        }

        public int NumberOfVisitors { get; set; }
    }
}
