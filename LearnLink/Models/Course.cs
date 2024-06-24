// Course.cs (Model)
using System.ComponentModel.DataAnnotations;

namespace LearnLink.Models
{
    public class Course
    {
        public int CourseID { get; set; }

        [Required]
        public string CourseName { get; set; }

        public int TeacherID { get; set; }

        // You can add more properties as needed
    }
}
