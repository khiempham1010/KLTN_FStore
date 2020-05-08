using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtomStore.Data.Entities
{
    public class Message
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
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime When { get; set; }

        public Guid? UserId { set; get; }

        public virtual AppUser AppUser { get; set; }

    }
}
