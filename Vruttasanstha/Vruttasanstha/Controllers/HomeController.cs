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
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Vrutasanastacon"].ConnectionString);
        List<SelectListItem> CompType = new List<SelectListItem>();
        List<SelectListItem> Country = new List<SelectListItem>();
        Vhelper v = new Vhelper();
        AdminController admin = new AdminController();
     

        public ActionResult Registration()
        {
            ViewBag.comptype = getNewsCompanytype();
            ViewBag.getCountry = getCountry();
            return View();
        }

        [HttpPost]
        public ActionResult Registration(NewUserRegister r, string btnRegister)
        {
            ViewBag.comptype = getNewsCompanytype();
            ViewBag.getCountry = getCountry();
            if (this.ModelState.IsValid)
            {
                if (r.acceptagri == true)
                {
                    if (btnRegister == "Create My Account")
                    {
                        TempData["message"] = r.SaveRegistration(r);
                        this.ModelState.Clear();
                        return View();
                    }
                    return View(r);
                }
                else
                    TempData["message"] = "Please accept Terms & Condition";
            }

            return View(r);
        }

        public List<SelectListItem> getNewsCompanytype()
        {
            DataTable dt = new DataTable();
            string query = "select CID,CompanyName,CompanyType from CompanyType";
            dt = Executequery(query);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    CompType.Add(new SelectListItem()
                    {
                        Text = dr["CompanyType"].ToString(),
                        Value = dr["CID"].ToString()
                    });
                }
            }
            return new List<SelectListItem>(CompType);
        }

        public List<SelectListItem> getCountry()
        {
            DataTable dt = new DataTable();
            string query = "select CountryID,CountryName,CountryCode from [dbo].[CountryMaster]";
            dt = Executequery(query);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Country.Add(new SelectListItem()
                    {
                        Text = dr["CountryName"].ToString(),
                        Value = dr["CountryID"].ToString()
                    });
                }
            }
            return new List<SelectListItem>(Country);
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

        public ActionResult Home()
        {
            Home h = new Home();
            DataTable dt = new DataTable();
            string query = "select * from News  where NCatId=0";
            dt = Executequery(query);
            h.home=GetNewslist(dt,true);
            var dt1 = new DataTable();
            query = "select top 10 * from News where NCatId=3 order by NcreatedDate desc";
            dt1 = Executequery(query);
            h.Geopolitical = GetGeoNewslist(dt1);


            var dt2 = new DataTable();
        string    query2 = "select top 1  * from News where NCatId=5 order by NcreatedDate desc";
            dt2 = Executequery(query2);
            h.EconomicalNews = GetGeoNewslistDefault(dt2);
            return View(h);
        }
        public List<Home> GetNewslist(DataTable dt,bool allow250)
        {
            List<Home> objupdatedata = new List<Home>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    byte[] arr = null;
                    if (dr["Nimage"] != DBNull.Value)
                    {
                        arr = (byte[])dr["Nimage"];
                    }

                    string Newsdesc = dr["Ndescription"].ToString();
                    string[] newsplitarr;
                    if (Newsdesc.Length > 250 && allow250 == true)
                    {

                        Newsdesc = Newsdesc.Substring(0, 250);
                        newsplitarr = Newsdesc.Split('#');
                        Newsdesc = newsplitarr[0];
                    }
                    else
                    {

                        newsplitarr = Newsdesc.Split('#');
                        string finalarray = "";
                        if (newsplitarr.Length > 0)
                        {
                            for (int i = 0; i < newsplitarr.Length; i++)
                            {
                                finalarray += newsplitarr[i];
                            }
                            Newsdesc = finalarray;
                        }
                        else
                            Newsdesc = newsplitarr[0];
                    }
                    objupdatedata.Add(new Home { NewsHeading = dr["NHeading"].ToString(), newsid = Convert.ToString(dr["NId"]), Description = Newsdesc, Createddate = Convert.ToString(dr["NcreatedDate"]), NewsImage = "data:image/jpg;base64," + Convert.ToBase64String(arr) });
                }
            }
            return objupdatedata;
        }


        public List<Home> GetGeoNewslist(DataTable dt)
        {
            List<Home> objupdatedata = new List<Home>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string Newsdesc = dr["Ndescription"].ToString();
                    if (Newsdesc.Length > 200)
                    Newsdesc = Newsdesc.Substring(0, 200);
                    objupdatedata.Add(new Home {GeoNewsid=Convert.ToInt32(dr["NId"]),GeoNewsDesc=Newsdesc, Newstime = dr["NcreatedDate"].ToString(), heading = dr["NHeading"].ToString() });
                }
            }
            return objupdatedata;
        }

        public List<Home> GetGeoNewslistDefault(DataTable dt)
        {
            List<Home> objupdatedata = new List<Home>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    byte[] arr = null;
                    if (dr["Nimage"] != DBNull.Value)
                    {
                        arr = (byte[])dr["Nimage"];
                    }

                    
                    string Newsdesc = dr["Ndescription"].ToString();
                    string[] newsplitarr;
                    if (Newsdesc.Length > 250)
                    {
                        Newsdesc = Newsdesc.Substring(0, 250);
                        newsplitarr = Newsdesc.Split('#'); 
                        Newsdesc = newsplitarr[0];
                    }
                    objupdatedata.Add(new Home { GeoNewsid = Convert.ToInt32(dr["NId"]), GeoNewsDesc = Newsdesc, Newstime = dr["NcreatedDate"].ToString(), heading = dr["NHeading"].ToString(), NewsImage = "data:image/jpg;base64," + Convert.ToBase64String(arr) });
                }
            }
            return objupdatedata;
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult WhoWeAre()
        {
            return View();
        }

        public ActionResult WhatWeServe()
        {
            return View();
        }

        public ActionResult Trust()
        {
            return View();
        }

        public ActionResult ExecutiveBoard()
        {
            return View();
        }

        public ActionResult EditorialBoard()
        {
            return View();
        }

        public ActionResult OrganizationProfile()
        {
            return View();
        }

        public ActionResult LegalDisclaimer()
        {
            return View();
        }

        public ActionResult CodeofConduct ()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult PrinciplesAndEthics()
        {
            return View();
        }
        public ActionResult AdminRegistration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminRegistration(Adminreg A, string btnsubmit)
        {
            if (this.ModelState.IsValid)
            {
                if (btnsubmit == "Register")
                {
                    string filepath = "";
                    byte[] imgarray = null;
                    if (A.Aimage != null && A.Aimage.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(A.Aimage.FileName);
                        string tsfilename = v.AppendTimeStamp(fileName);
                        filepath = Path.Combine(Server.MapPath("~/images"), tsfilename);
                        A.Aimage.SaveAs(filepath);
                        imgarray = v.ImageToByteArray(filepath);
                    }
                    ViewBag.message = v.SaveAdminRegistration(A, imgarray);
                    this.ModelState.Clear();
                    return View(A);
                }
            }
            return View(A);
        }
        public string GetNews(string NId)
        {
            DataTable dt = new DataTable();
            string str = string.Empty;
            string query = "select * from News where NId=" + NId;
            str = admin.GetseriliseResult(Executequery(query));
            return str;
        }
        public string GetNews1(string NId)
        {
            DataTable dt = new DataTable();
            string str = string.Empty;
            string query = "select * from News where NId=5";
            str = admin.GetseriliseResult(Executequery(query));
            return str;
        }
        public ActionResult ReadMore(string id)
        {
            Home h = new Home();
            DataTable dt = new DataTable();
            string query = "select * from News where NId="+Convert.ToInt32(id);
            dt = Executequery(query);
            h.home = GetNewslist(dt,false);
            return View(h);
        }
        public ActionResult Signout()
        {
            Session.Clear();
            Session.RemoveAll();
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }
    }
}