using MyApp.School.Domains;
using System.ComponentModel.DataAnnotations;

namespace MyApp.School.Public.Dtos
{
    public class StudentEnrollmentListDto
    {
        public string CourseTitle { get; set; }

        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }

    }

}
