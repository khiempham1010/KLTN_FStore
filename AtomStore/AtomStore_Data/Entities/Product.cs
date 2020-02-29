using AtomStore.Data.Enums;
using AtomStore.Data.Interfaces;
using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AtomStore.Data.Entities
{
    [Table("Products")]
    public class Product : DomainEntity<int>, ISwitchable, IHasSoftDelete, IHasSeoMetaData, IDateTracking
    {
        public Product()
        {
            ProductTags = new List<ProductTag>();
        }

        public Product(string name, int categoryId, string thumbnailImage,
            decimal price, decimal originalPrice, decimal? promotionPrice,
            string description, string content, bool? homeFlag,
            string tags, string unit, Status status, string seoPageTitle,
            string seoAlias, string seoMetaKeyword,
            string seoMetaDescription,DateTime dateCreated,DateTime dateModified)
        {
            Name = name;
            CategoryId = categoryId;
            Image = thumbnailImage;
            Price = price;
            OriginalPrice = originalPrice;
            PromotionPrice = promotionPrice;
            Description = description;
            Content = content;
            HomeFlag = homeFlag;
            Tags = tags;
            Unit = unit;
            Status = status;
            SeoPageTitle = seoPageTitle;
            SeoAlias = seoAlias;
            SeoKeywords = seoMetaKeyword;
            SeoDescription = seoMetaDescription;
            DateCreated = dateCreated;
            DateModified = dateModified;
            ProductTags = new List<ProductTag>();

        }

        public Product(int id, string name, int categoryId, string thumbnailImage,
             decimal price, decimal originalPrice, decimal? promotionPrice,
             string description, string content, bool? homeFlag,
             string tags, string unit, Status status, string seoPageTitle,
             string seoAlias, string seoMetaKeyword,
             string seoMetaDescription, DateTime dateCreated, DateTime dateModified)
        {
            Id = id;
            Name = name;
            CategoryId = categoryId;
            Image = thumbnailImage;
            Price = price;
            OriginalPrice = originalPrice;
            PromotionPrice = promotionPrice;
            Description = description;
            Content = content;
            HomeFlag = homeFlag;
            Tags = tags;
            Unit = unit;
            Status = status;
            SeoPageTitle = seoPageTitle;
            SeoAlias = seoAlias;
            SeoKeywords = seoMetaKeyword;
            SeoDescription = seoMetaDescription;
            DateCreated = dateCreated;
            DateModified = dateModified;
            ProductTags = new List<ProductTag>();

        }
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
        public string  Tags { get; set; }
        public string  Unit { get; set; }
        [ForeignKey("CategoryId")]
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductTag> ProductTags { set; get; }
        public Status Status { set; get; }
        public bool IsDeleted { set; get; }
        [StringLength(255)]
        public string SeoPageTitle { set; get; }

        [Column(TypeName ="Varchar(255)")]
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
