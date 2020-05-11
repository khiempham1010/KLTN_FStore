using AtomStore.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtomStore.Data.Entities
{
    public class Message: DomainEntity<int>
    {
        public Message()
        {

        }
        public Message(int id, string name, string text, DateTime when,Guid? userId)
        {
            Id = id;
            Name = name;
            Text = text;
            When = when;
            UserId = userId;
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime When { get; set; }

        public Guid? UserId { set; get; }

        public virtual AppUser AppUser { get; set; }

    }
}
