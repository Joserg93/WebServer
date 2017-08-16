using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WebServer.Models
{
    public class Account
    {
        public int id { get; set; }        
        public string name { get; set; }       
        public string date { get; set; }       
        public string text { get; set; }        
        public int user_id { get; set; }
    }

    public class User
    {       
        public string email { get; set; }
        public string password { get; set; }
    }
    public class Register
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
    }
}