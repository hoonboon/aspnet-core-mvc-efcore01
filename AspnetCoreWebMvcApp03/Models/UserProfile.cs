using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreWebMvcApp03.Areas.Identity.Data
{
    public class UserProfile : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

    }
}
