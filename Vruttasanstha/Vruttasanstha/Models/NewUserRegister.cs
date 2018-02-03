using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Vruttasanstha.Models
{
    public class NewUserRegister
    {
        Vhelper v = new Vhelper();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Vrutasanastacon"].ConnectionString);
        public int rId { get; set; }

        [Required(ErrorMessage="Enter First Name")]
        public string fname { get; set; }
       
                 
        [Required(ErrorMessage="Enter Last Name")]
        public string lname { get; set; }

        [Required(ErrorMessage = "Enter Company Name")]
        public string cmpname { get; set; }

        public int Comptype { get; set; }

        [Required(ErrorMessage = "Enter Email Address")]
        public string emailid { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        public string password { get; set; }

        [Required(ErrorMessage = "Please Confirm  Password")]
        public string confirmPass { get; set; }

        [Required(ErrorMessage = "Enter Business Phone")]
        public string BPhone { get; set; }

        [Required(ErrorMessage = "Enter Secret Answer")]
        public string secretAnswer { get; set; }

        public string Countryname { get; set; }
        public string questionname { get; set; }


        public bool Photo { get; set; }

        public bool Videoarchieve { get; set; }

        public bool VideoNewsFeedServices { get; set; }

        public bool  NewsProductionSystem { get; set; }
        public bool Audio { get; set; }
        public bool Text { get; set; }
        public bool Graphics { get; set; }
        public bool Interactives { get; set; }
        public bool Assignments { get; set; }

        public bool Other { get; set; }

        public bool acceptagri { get; set; }

      /*===============*/

        public bool Marathi { get; set; }
        public bool English { get; set; }
        public bool GeopolyticalDesk { get; set; }
        public bool ruralagri { get; set; }
        public bool economy { get; set; }
        public bool image { get; set; }

        public bool audiovisual { get; set; }

        public string SaveRegistration(NewUserRegister r)
        {
            string message = string.Empty; 

          SqlCommand cmd = new SqlCommand("sp_register");
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Fname", SqlDbType.NVarChar).Value =r.fname;
                cmd.Parameters.Add("@Lname", SqlDbType.NVarChar).Value = r.lname;
                cmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = r.cmpname;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = r.emailid;
                cmd.Parameters.Add("@C_type", SqlDbType.Int).Value = r.Comptype;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value =v.Encryptdata(r.password);
                cmd.Parameters.Add("@C_Password", SqlDbType.NVarChar).Value =v.Encryptdata(r.confirmPass);
                cmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = r.BPhone;
                cmd.Parameters.Add("@Country_Id", SqlDbType.NVarChar).Value =r.Countryname;
                cmd.Parameters.Add("@Q_Id", SqlDbType.NVarChar).Value = r.questionname;


             cmd.Parameters.Add("@Q_answer", SqlDbType.NVarChar).Value = r.secretAnswer;
                cmd.Parameters.Add("@photo", SqlDbType.Int).Value =CheckBool(r.Photo);
                cmd.Parameters.Add("@videonewsfeedservice", SqlDbType.Int).Value = CheckBool(r.VideoNewsFeedServices);
                cmd.Parameters.Add("@audio", SqlDbType.Int).Value = CheckBool(r.Audio);
                cmd.Parameters.Add("@graphics", SqlDbType.Int).Value = CheckBool(r.Graphics);
                cmd.Parameters.Add("@assignments", SqlDbType.Int).Value = CheckBool(r.Assignments);

                cmd.Parameters.Add("@videoarchieve", SqlDbType.Int).Value = CheckBool(r.Videoarchieve);
                cmd.Parameters.Add("@newsproductionsys", SqlDbType.Int).Value = CheckBool(r.NewsProductionSystem);
                cmd.Parameters.Add("@text", SqlDbType.Int).Value = CheckBool(r.Text);
                cmd.Parameters.Add("@interactive", SqlDbType.Int).Value = CheckBool(r.Interactives);
                cmd.Parameters.Add("@other", SqlDbType.Int).Value = CheckBool(r.Other);
                cmd.Parameters.Add("@marathi", SqlDbType.Int).Value = CheckBool(r.Marathi);
                cmd.Parameters.Add("@english", SqlDbType.Int).Value = CheckBool(r.English);
                cmd.Parameters.Add("@geopoly", SqlDbType.Int).Value = CheckBool(r.GeopolyticalDesk);
                cmd.Parameters.Add("@ruralagri", SqlDbType.Int).Value = CheckBool(r.ruralagri);
                cmd.Parameters.Add("@economy", SqlDbType.Int).Value = CheckBool(r.economy);
                cmd.Parameters.Add("@images", SqlDbType.Int).Value = CheckBool(r.image);
                cmd.Parameters.Add("@audiovisual", SqlDbType.Int).Value = CheckBool(r.audiovisual);

                con.Open();
                cmd.ExecuteNonQuery();
                message= "register successfully...!!!";
            return message;
        }

        public int CheckBool(bool name)
        {
            int i = 0;
            if (name == true)
                i = 1;
            else if(name==false)
                i = 0;

            return i;
        }
    }
}