using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtomStore.Data.Entities
{
    public class SizeType: DomainEntity<int>
    {
        public SizeType()
        {
            Sizes = new List<Size>();
        }
        [StringLength(250)]
        public string Name { get; set; }
        public ICollection<Size> Sizes { get; set; }
    }
}
