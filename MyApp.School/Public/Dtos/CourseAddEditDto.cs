using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.School.Public.Dtos
{
    public class CourseAddEditDto
    {
        [Display(Name = "Course Number")]
        public int CourseId { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(0, 5)]
        public int Credits { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public override string ToString()
        {
            return $"CourseId={CourseId}, Title={Title}, Credits={Credits}, DepartmentId={DepartmentId}";
        }
    }
}
