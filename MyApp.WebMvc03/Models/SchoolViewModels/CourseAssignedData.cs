using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Models.SchoolViewModels
{
    public class CourseAssignedData
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public bool IsAssigned { get; set; }
    }
}
