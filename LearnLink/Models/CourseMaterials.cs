using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class CourseMaterials
    {
        public int FileID { get; set; }    
        public int CourseID { get; set; }      
        public int TeacherID { get; set; }
        public string Name { get; set; }       
        public string ContentType { get; set; } 
        public byte[] Data { get; set; }
        public DateTime UploadDate { get; set; }
    }
}