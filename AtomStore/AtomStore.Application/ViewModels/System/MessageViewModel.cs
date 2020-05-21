using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtomStore.Application.ViewModels.System
{
    public class MessageViewModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime When { get; set; }
        public Guid UserId { set; get; }
        public virtual AppUserViewModel AppUser { get; set; }
        public Guid ReceiverId { set; get; }
    }
}
