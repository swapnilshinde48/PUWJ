using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vruttasanstha.Models;

namespace Vruttasanstha.Controllers
{
    public class UserController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Vrutasanastacon"].ConnectionString);
        AdminController a = new AdminController();
        Vhelper v = new Vhelper();
        //
        // GET: /User/
        public ActionResult User()
        {
            if (Session["RegId"] != null)
            {
                ViewBag.name = Session["RegId"].ToString();
                
            //select * from Register r inner join Client_Subcription c on r.RegId=c.RegId inner join News n on c.SubId=n.NCatId  where r.RegId= Convert.ToInt32(Session["RegId"])
            //DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            UserNews un = new UserNews();
            un.regid = Session["RegId"].ToString();
            //string query = "select * from Register r inner join Client_Subcription c on r.RegId=c.RegId inner join News n on c.SubId=n.NCatId  where r.RegId=" + Convert.ToInt32(Session["RegId"]);
            //DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("sp_Retrieve_Top1_News");
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@NCatId", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@RegId", SqlDbType.Int).Value = Convert.ToInt32(Session["RegId"]);
            con.Open();
            cmd.ExecuteNonQuery();
            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            ad.Fill(dt1);
            con.Close();
            if (dt1.Rows.Count > 0)
            {
                un.nid = Convert.ToInt32(dt1.Rows[0]["NId"]);
                un.nheading = dt1.Rows[0]["NHeading"].ToString();
                un.description = dt1.Rows[0]["NHeading"].ToString();
                un.date = dt1.Rows[0]["NcreatedDate"].ToString();
                un.author = dt1.Rows[0]["NCreatedby"].ToString();

                if (dt1.Rows[0]["Nimage"] != DBNull.Value)
                {

                    byte[] arr = (byte[])dt1.Rows[0]["Nimage"];
                    un.image = "data:image/jpg;base64," + Convert.ToBase64String(arr);
                }

            }
            un.unews = GetlistData(GetNewsfromSp(Session["RegId"].ToString(), "1"));
            return View(un);

            }
            else { return RedirectToAction("Login", "Login"); }
        }
        public DataTable Executequery(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter cmd_Ad = new SqlDataAdapter(query, con);
                con.Open();
                cmd_Ad.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception ex) { throw ex; }
        }
        public List<UserNews> GetlistData(DataTable dt)
        {
            List<UserNews> objupdatedata = new List<UserNews>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    objupdatedata.Add(new UserNews { NID = Convert.ToInt32(dr["NId"]), newsheading = dr["NHeading"].ToString()});
                }
            }
            return objupdatedata;
        }
        public string GetNews(string NId)
        {
            DataTable dt = new DataTable();
            string str = string.Empty;
            string query = "select * from News where NId=" + NId;
            str = a.GetseriliseResult(Executequery(query));
            return str;
        }
        public string GetNewsonCategory(string id, string Catid)
        {
            DataTable dt = new DataTable();
            string str = string.Empty;
//            string query = @"select * from Register r inner join Client_Subcription c on r.RegId=c.RegId inner join News n on c.SubId=n.NCatId 
//                              where r.RegId="+id+"and NCatId="+Catid;

            //string query = @"select * from news n inner join NewsCategory c on n.NCatId=c.NCatId where n.NCatId=" + Catid;

       
            str = a.GetseriliseResult(GetNewsfromSp( id,  Catid));

            return str;
        }

        public DataTable GetNewsfromSp(string id, string Catid)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("sp_Retrieve_News");
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@NCatId", SqlDbType.Int).Value = Convert.ToInt32(Catid);
            cmd.Parameters.Add("@RegId", SqlDbType.Int).Value = Convert.ToInt32(id);
            con.Open();
            cmd.ExecuteNonQuery();
            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            ad.Fill(dt);
            con.Close();
            return dt;
        }

        public string GetCategories(string id)
        {
            string str = string.Empty;
            string query = "select * from [dbo].[Client_Subcription] where RegId=" + id;
            str = a.GetseriliseResult(Executequery(query));
            return str;
        }

        public FileResult Download(string id)
        {
     
            string contentType = "video/mp4";
            //Parameters to file are
            //1. The File Path on the File Server
            //2. The content type MIME type
            //3. The parameter for the file save by the browser
            return File(Server.MapPath("~/video/"), contentType, id);
        }
        public ActionResult DownloadVideoNews(string filename)
        {

            string path = Server.MapPath("~/Video");
            string file = Path.Combine(path, filename);
            file = Path.GetFullPath(file);
            if (!file.StartsWith(path))
            {
                throw new HttpException(403, "Forbidden");
            }
            return File(file, "video/mp4");
        }
        public ActionResult DownloadAudioNews(string filename)
        {

            string path = Server.MapPath("~/Audio");
            string file = Path.Combine(path, filename);
            file = Path.GetFullPath(file);
            if (!file.StartsWith(path))
            {
                throw new HttpException(403, "Forbidden");
            }
            return File(file, "audio/mp3");
        }
        public ActionResult Changepassword()
        {
            if (Session["RegId"] != null && Session["User"] != null)
            {
                ViewBag.name = Session["User"].ToString();
                return View();
            }
            else { return RedirectToAction("Login", "Login"); }
        }
        [HttpPost]
        public ActionResult Changepassword(Changepassword c,string btnsubit)
        {
            if (Session["RegId"] != null && Session["User"] != null)
            {
                if (this.ModelState.IsValid)
                {
                    DataTable dt = new DataTable();
                    string str = string.Empty;
                    str = "select * from Register where RegId=" + Convert.ToInt32(Session["RegId"])+" and Password='"+v.Encryptdata(c.OldPassword)+"'";
                    dt = v.Executequery(str);
                    if (dt.Rows.Count > 0)
                    {
                        DataTable dt1 = new DataTable();
                        string query = "update Register set Password='" + v.Encryptdata(c.NewPassword) + "',C_Password='" + v.Encryptdata(c.ConfirmPassword) + "' where RegId=" + Convert.ToInt32(Session["RegId"]) + " and Password='" + v.Encryptdata(c.OldPassword) + "'";
                        dt1 = v.Executequery(query);
                        if (dt1.Rows.Count > 0)
                        {
                        }
                        return RedirectToAction("Login", "Login");
                    }
                    else
                    {
                        ViewBag.message = "incorrect old password";
                        return View(c);
                    }
                }
                else return View(c);
            }
            else { return RedirectToAction("Login", "Login"); }
        }
	}
}