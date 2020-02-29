using AtomStore.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtomStore.Application.ViewModels.Product
{
    public class ProductCategoryViewModel
    {
        public int Id { get; set; }
        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }
        public int? HomeOrder { get; set; }
        public bool? HomeFlag { get; set; }
        [StringLength(255)]
        public string SeoPageTitle { set; get; }
        [StringLength(255)]
        public string SeoAlias { set; get; }
        [StringLength(255)]
        public string SeoKeywords { set; get; }
        [StringLength(255)]
        public string SeoDescription { set; get; }
        public Status Status { set; get; }
        public int SortOrder { set; get; }
        public DateTime DateCreated { set; get; }
        public DateTime DateModified { set; get; }
        public ICollection<ProductViewModel> Products { get; set; }
    }
}
