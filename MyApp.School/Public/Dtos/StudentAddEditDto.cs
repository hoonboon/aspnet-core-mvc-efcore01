using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.School.Public.Dtos
{
    public class StudentAddEditDto
    {
        public int StudentId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9\s]*$", ErrorMessage = "Please enter alphanumeric value, starting with capital letter.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9\s]*$", ErrorMessage = "Please enter alphanumeric value, starting with capital letter.")]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }

        public override string ToString()
        {
            return $"StudentId={StudentId}, LastName={LastName}, FirstMidName={FirstMidName}, " 
                + $"EnrollmentDate={EnrollmentDate}";
        }
    }
}
