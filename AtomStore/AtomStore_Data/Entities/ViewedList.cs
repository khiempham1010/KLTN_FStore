using AtomStore.Data.Interfaces;
using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AtomStore.Data.Entities
{
    public class ViewedList : DomainEntity<int>, IDateTracking
    {
        public ViewedList(int id, Guid userId, int productId, string email, string productName, DateTime dateCreated, DateTime dateModified)
        {
            Id = id;
            UserId = userId;
            ProductId = productId;
            Email = email;
            ProductName = productName;
            DateCreated = dateCreated;
            DateModified = dateModified;
        }
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { set; get; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
