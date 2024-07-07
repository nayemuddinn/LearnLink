using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class CourseMaterials
    {
        public int MaterialID { get; set; }
        public int CourseID { get; set; }
        public string MaterialName { get; set; }
        public string Content { get; set; }
    }
}