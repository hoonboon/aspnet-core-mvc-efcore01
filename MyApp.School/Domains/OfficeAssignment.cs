using System.ComponentModel.DataAnnotations;

namespace MyApp.School.Domains
{
    public class OfficeAssignment
    {
        [Key]
        public int InstructorId { get; set; }

        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        public Instructor Instructor { get; set; }
    }

}