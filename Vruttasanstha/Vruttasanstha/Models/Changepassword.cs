using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Vruttasanstha.Models
{
    public class Changepassword
    {
        [Required(ErrorMessage = "Required old password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Required new password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Required confirm password")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}