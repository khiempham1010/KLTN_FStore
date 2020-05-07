using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtomStore.Application.ViewModels.Product
{
    public class SizeTypeViewModel
    {
        public int Id { get; set; }
        [StringLength(250)]
        public string Name { get; set; }

        //public ICollection<SizeViewModel> Sizes { get; set; }
    }
}
