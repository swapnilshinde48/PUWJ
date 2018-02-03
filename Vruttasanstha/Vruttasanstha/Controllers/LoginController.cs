using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vruttasanstha.Models;

namespace Vruttasanstha.Controllers
{
    public class LoginController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Vrutasanastacon"].ConnectionString);
        Vhelper v = new Vhelper();
        //
        // GET: /Login/
        public ActionResult Signout()
        {
            Session.Clear();
            Session.RemoveAll();
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }
        public ActionResult Login()
        {
            Login m = new Login();
            Signout();
            DataTable dt = new DataTable();
            string  query = "select top 10 * from News where NCatId=4 order by NcreatedDate desc";
            dt = Executequery(query);
            ViewBag.news =dt;
            DataTable dt1 = new DataTable();
            string query1 = "select top 10 * from News where NCatId=1 order by NcreatedDate desc";
            dt1 = Executequery(query1);
            ViewBag.marathinews = dt1;
            return View(m);
        }

        [HttpPost]
        public ActionResult Login(Login l)
        {
            if (this.ModelState.IsValid)
            {
                DataTable dt1 = new DataTable();

                string query = "select (Afname+' ' + Alname)as Name,Ausername,Asuperadmin,Aemail,Aminid,Apass from tAdmin where Aemail='" + l.username + "' and Apass='" + v.Encryptdata(l.password) + "'";
                dt1 = Executequery(query);
                if (dt1.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt1.Rows)
                    {
                        if (l.username == dr["Aemail"].ToString() && l.password == v.Decryptdata(dr["Apass"].ToString()))
                        {
                            Session["Adminname"] = dr["Name"].ToString();
                            Session["adminemail"] = dr["Aemail"].ToString();
                            Session["adiminRegId"] = dr["Aminid"].ToString();
                            return RedirectToAction("admin", "admin");
                        }
                    } return View(l);
                }
                else
                {
                    DataTable dt = new DataTable();
                    string user = "select (Fname+' ' + Lname)as Name,Email,RegId,Password from Register where Email='" + l.username + "' and Password='" + v.Encryptdata(l.password) + "'";
                    dt = Executequery(user);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (l.username == dr["Email"].ToString() && l.password == v.Decryptdata(dr["Password"].ToString()))
                            {
                                Session["User"] = dr["Name"].ToString();
                                Session["Email"] = dr["Email"].ToString();
                                Session["RegId"] = dr["RegId"].ToString();
                                return RedirectToAction("user", "user");
                            }
                        }
                        return View(l);
                    }
                    else
                    {
                        ViewBag.message = "Incorrect Username or Password";
                        DataTable dt2 = new DataTable();
                        string query1 = "select top 10 * from News where NCatId=4 order by NcreatedDate desc";
                        dt2 = Executequery(query1);
                        ViewBag.news = dt2;
                        DataTable dt3 = new DataTable();
                        string query3 = "select top 10 * from News where NCatId=1 order by NcreatedDate desc";
                        dt3 = Executequery(query3);
                        ViewBag.marathinews = dt3;
                    }

                }
            }
            return View(l);
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

        public ActionResult ChangePassword()
        {
            return View();
        }
	}
}