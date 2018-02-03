using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
///using System.Web.Configuration;
using System.Configuration;
namespace Vruttasanstha.Models
{
    public class uploadnew
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Vrutasanastacon"].ConnectionString);
        public int NId { get; set; }
        public string heading { get; set; }
        public string fname { get; set; }
        public string category { get; set; }
        public int catid { get; set; }
        public string discription { get; set; }
        public string posteddate { get; set; }

        public string uploadedby { get; set; }

        public HttpPostedFileBase NLImage1 { get; set; }
        public HttpPostedFileBase NLImage { get; set; }

        public string Videoaudio { get; set; }
        public string Videoaudio1 { get; set; }
        public string image { get; set; }

        public bool Videoarchieve { get; set; }
        public bool NewsProductionSystem { get; set; }
        public bool Text { get; set; }
        public bool Interactives { get; set; }
        public bool Other { get; set; }
        
        public int rid { get; set; }
        public string user { get; set; }

        public int hiddenid { get; set; }
        public int uphiddenid { get; set; }
       public List<uploadnew> un = new List<uploadnew>();
       public List<uploadnew> un1 = new List<uploadnew>();
       public bool Marathi { get; set; }
       public bool English { get; set; }
       public bool GeoPolitical { get; set; }
       public bool Financial { get; set; }
       public bool Management { get; set; }
       public bool Legal { get; set; }
       public bool Sports { get; set; }
       public bool Research { get; set; }
       public bool Education { get; set; }
       public bool Testagainn { get; set; }
       public bool Images { get; set; }
       public bool Videos { get; set; }

       public bool Ruralagri { get; set; }
       public bool economical { get; set; }

       public string createdDate { get; set; }

       public string heading1 { get; set; }
       public string category1 { get; set; }
       public int catid1 { get; set; }
       public string discription1 { get; set; }

       public string Savesubscription(uploadnew r)
       {
           string message = string.Empty;

           SqlCommand cmd = new SqlCommand("sp_register");
           cmd.Connection = con;
           cmd.CommandType = CommandType.StoredProcedure;
           cmd.Parameters.Add("@Fname", SqlDbType.NVarChar).Value = r.fname;
           cmd.Parameters.Add("@videoarchieve", SqlDbType.Int).Value = CheckBool(r.Videoarchieve);
           cmd.Parameters.Add("@newsproductionsys", SqlDbType.Int).Value = CheckBool(r.NewsProductionSystem);
           cmd.Parameters.Add("@text", SqlDbType.Int).Value = CheckBool(r.Text);
           cmd.Parameters.Add("@interactive", SqlDbType.Int).Value = CheckBool(r.Interactives);
           cmd.Parameters.Add("@other", SqlDbType.Int).Value = CheckBool(r.Other);

           con.Open();
           cmd.ExecuteNonQuery();
           message = "register successfully...!!!";
           return message;
       }

       public int CheckBool(bool name)
       {
           int i = 0;
           if (name == true)
               i = 1;
           else if (name == false)
               i = 0;

           return i;
       }
    }
}