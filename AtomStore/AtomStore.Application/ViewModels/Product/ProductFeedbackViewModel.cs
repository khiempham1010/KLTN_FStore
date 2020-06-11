using AtomStore.Application.ViewModels.System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AtomStore.Application.ViewModels.Product
{
    public class ProductFeedbackViewModel
    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [StringLength(255)]
        [Required]
        public string Title { get; set; }
        [StringLength(255)]
        [Required]
        public string Content { get; set; }
        [Required]
        public Decimal Rating { get; set; }
        public int ParentId { get; set; }
        public int Like { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        [Required]
        public Guid OwnerId { get; set; }
        public virtual AppUserViewModel AppUser { get; set; }
        public virtual ProductViewModel Product { get; set; }
        public string Image { get; set; }
    }
}
