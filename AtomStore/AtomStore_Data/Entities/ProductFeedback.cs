using AtomStore.Data.Interfaces;
using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AtomStore.Data.Entities
{
    [Table("ProductFeedbacks")]
    public class ProductFeedback : DomainEntity<int>, IDateTracking, IHasOwner<Guid>
    {
        public ProductFeedback(
            int productId,
            string title,
            string content,
            Decimal rating,
            int parentId,
            DateTime dateCreated,
            DateTime dateModified,
            Guid ownerId,
            int like,
            string image)
        {
            ProductId = productId;
            Title = title;
            Content = content;
            Rating = rating;
            ParentId = parentId;
            DateCreated = dateCreated;
            DateModified = dateModified;
            OwnerId = ownerId;
            Like = like;
            Image = image;
        }
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
        [ForeignKey("OwnerId")]
        public virtual AppUser AppUser { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public string Image { get; set; }
    }
}
