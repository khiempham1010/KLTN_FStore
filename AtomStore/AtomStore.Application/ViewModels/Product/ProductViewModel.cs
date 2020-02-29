using AtomStore.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtomStore.Application.ViewModels.Product
{
    public class ProductViewModel
    {

        public int Id { get; set; }
        [StringLength(255)]
        [Required]
        public string Name { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        [DefaultValue(0)]
        public decimal Price { get; set; }
        [StringLength(255)]
        public string Image { get; set; }
        public decimal? PromotionPrice { get; set; }
        [Required]
        public decimal OriginalPrice { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public string Content { get; set; }
        public bool? HomeFlag { get; set; }
        public int? ViewCount { get; set; }
        [StringLength(255)]
        public string Tags { get; set; }
        public string Unit { get; set; }
        public virtual ProductCategoryViewModel ProductCategory { get; set; }
        public Status Status { set; get; }
        public bool IsDeleted { set; get; }
        [StringLength(255)]
        public string SeoPageTitle { set; get; }

        [StringLength(255)]
        public string SeoAlias { set; get; }
        [StringLength(255)]
        public string SeoKeywords { set; get; }
        [StringLength(255)]
        public string SeoDescription { set; get; }
        public DateTime DateCreated { set; get; }
        public DateTime DateModified { set; get; }
    }
}
