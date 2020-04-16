using AtomStore.Application.ViewModels.System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AtomStore.Models
{
    public class AccountSettingsViewModel
    {
        public AppUserViewModel AppUserViewModel { get; set; }

        [Display(Name = "Full name")]
        public string FullName { set; get; }

        [Display(Name = "DOB")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { set; get; }

       
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { set; get; }

        [Display(Name = "Avatar")]
        public string Avatar { get; set; }
    }
}
