using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vruttasanstha.Models
{
    public class Login
    {
        public int id { get; set; }
        [Display(Name = "Email Id")]
        [Required(ErrorMessage = "Enter your Email-ID")]
        public string username { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter Valid Password")]
        public string password { get; set; }
        //[Required(ErrorMessage = "Please Select Registration Type")]
        //public string getchecked { get; set; }

    }
}