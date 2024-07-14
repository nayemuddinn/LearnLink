using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LearnLink.Models
{
    public class GoogleProfile
    {
        public int UserID { get; set; }
        public string Name { get; set; }
       public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Institution { get; set; }
        public string Role { get; set; }

        public string UserRole { get; set; } // new

    }
}