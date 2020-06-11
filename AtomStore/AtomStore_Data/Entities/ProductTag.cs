using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AtomStore.Data.Entities
{
    public class ProductTag:DomainEntity<int>
    {
        public int ProductId { get; set; }
        public string TagId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("TagId")]
        public  virtual Tag Tag { get; set; }
    }
}
