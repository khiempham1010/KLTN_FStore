using AtomStore.Data.Interfaces;
using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AtomStore.Data.Entities
{
    [Table("FeedbackImage")]
    public class FeedbackImage : DomainEntity<int>
    {
        public int ProductFeedbackId { get; set; }

        [ForeignKey("ProductFeedbackId")]
        public virtual ProductFeedback ProductFeedback { get; set; }
        [StringLength(250)]
        public string Path { get; set; }

        [StringLength(250)]
        public string Caption { get; set; }
    }
}
