using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.School.Public.Dtos
{
    public class CourseDetailDto
    {
        [Display(Name = "Course Number")]
        public int CourseId { get; set; }

        public string Title { get; set; }

        public int Credits { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

    }
}
