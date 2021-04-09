using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.Admin.Security.Domains
{
    public class CacheControl
    {
        [Key]
        [Required]
        [MaxLength(60)]
        public string CacheKey { get; set; }

        public long LastRefreshTimeUtc { get; set; }
    }
}
