using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Models.SchoolViewModels
{
    public class StudentCountByEnrollmentDate
    {
        public DateTime? EnrollmentDate { get; set; }

        public int StudentCount { get; set; }
    }
}
