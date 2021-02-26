using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Areas.Identity.Data
{
    public class UserProfile : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        [PersonalData]
        public byte[] ProfilePicture { get; set; }
    }
}
