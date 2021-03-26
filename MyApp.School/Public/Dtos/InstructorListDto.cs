using MyApp.School.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.School.Public.Dtos
{
    public class InstructorListDto
    {
        public IEnumerable<InstructorListItem> Instructors { get; set; }
        public IEnumerable<CourseListItem> Courses { get; set; }
        public IEnumerable<EnrollmentListItem> Enrollments { get; set; }
    }

    public class InstructorListItem
    {
        public int InstructorId { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [Display(Name = "Office")]
        public string OfficeLocation { get; set; }

        [Display(Name = "Courses")]
        public IEnumerable<CourseListItem> CoursesAssigned { get; set; }
    }

    public class CourseListItem
    {
        public int CourseId { get; set; }

        public string CourseTitle { get; set; }

        public string DepartmentName { get; set; }
    }

    public class EnrollmentListItem
    {
        public string StudentName { get; set; }

        public Grade? StudentEnrollmentGrade { get; set; }
    }



}
