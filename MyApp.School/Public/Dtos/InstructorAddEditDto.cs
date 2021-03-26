using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.School.Public.Dtos
{
    public class InstructorAddEditDto
    {
        public int InstructorId { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [Display(Name = "Courses Assigned")]
        public IEnumerable<int> CoursesAssigned { get; set; }

        [Display(Name = "Office Location")]
        public string OfficeLocation { get; set; }

        public override string ToString()
        {
            return $"InstructorId={InstructorId}, LastName={LastName}, FirstMidName={FirstMidName}, " 
                + $"HireDate={HireDate}, CoursesAssigned={string.Concat(CoursesAssigned)}, "
                + $"OfficeLocation={OfficeLocation}";
        }
    }
}
