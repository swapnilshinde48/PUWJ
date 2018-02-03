using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Vruttasanstha.Models
{
    public class Adminreg
    {
           [Required(ErrorMessage = "Enter Firstname")]
        public string Afname{get;set;}

           [Required(ErrorMessage = "Enter lastname")]
         public string Alname{get;set;}

         [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
         [Required(ErrorMessage = "Enter Valid Email Address")]
         public string Aemail{get;set;}

         public string Acontactno{get;set;}
        [Required(ErrorMessage="Please Select Image to upload")]
         public HttpPostedFileBase Aimage { get; set; }

          [Required(ErrorMessage = "Enter Password")]
         public string Apass{get;set;}

      [Required(ErrorMessage = "Enter Password")]
      [Compare("Apass")]
         public string Acpass{get;set;}
             [Required(ErrorMessage = "Enter Username")]
         public string Ausername{get;set;}

         public int Asuperadmin{get;set;}   
    }
}