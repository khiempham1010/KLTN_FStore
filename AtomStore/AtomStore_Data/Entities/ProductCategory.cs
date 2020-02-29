using AtomStore.Data.Enums;
using AtomStore.Data.Interfaces;
using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtomStore.Data.Entities
{
    public class ProductCategory : DomainEntity<int>, IHasSeoMetaData, ISwitchable,ISortable,IDateTracking
    {
        public ProductCategory()
        {
            Products = new List<Product>();
        }

        public ProductCategory(string name, int? parentId, int? homeOrder , bool? homeFlag, int sortOrder, Status status, string seoPageTitle, string seoAlias,
            string seoKeywords, string seoDescription)
        {
            Name = name;
            ParentId = parentId;
            HomeOrder = homeOrder;
            HomeFlag = homeFlag;
            SortOrder = sortOrder;
            Status = status;
            SeoPageTitle = seoPageTitle;
            SeoAlias = seoAlias;
            SeoKeywords = seoKeywords;
            SeoDescription = seoDescription;
        }
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
        public virtual ICollection<Product> Products { get; set; }

    }
}
