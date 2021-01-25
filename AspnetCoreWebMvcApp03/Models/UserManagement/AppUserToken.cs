﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreWebMvcApp03.Areas.Identity.Data
{
    public class AppUserToken : IdentityUserToken<string>
    {
        public virtual AppUser User { get; set; }
    }
}
