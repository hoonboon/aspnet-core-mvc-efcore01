using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.School.Public.Dtos
{
    public class DepartmentDetailDto
    {
        public int DepartmentId { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,0.00}", ApplyFormatInEditMode = true)]
        public decimal Budget { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Administrator")]
        public string InstructorName { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
