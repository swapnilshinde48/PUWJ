using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vruttasanstha.Models;

namespace Vruttasanstha.Controllers
{
    public class AdminController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Vrutasanastacon"].ConnectionString);
        List<SelectListItem> NewsCat = new List<SelectListItem>();
        List<SelectListItem> NewsCatAll = new List<SelectListItem>();
        Vhelper v = new Vhelper();
        //
        // GET: /Admin/Session["adiminRegId"]
        public ActionResult Admin()
        {
           
            if (Session["adiminRegId"] != null)
            {
                ViewBag.name = Session["adiminRegId"].ToString();
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            ViewBag.cat = getNewsCat();
            ViewBag.Getallcat = getNewsCatAll();
            uploadnew u = new uploadnew();
            string query = "select * from Register order by RegId";
            dt = Executequery(query);
            u.un = GetlistData(dt);
             string query1 = "select * from News order by NId desc";
             dt1 = Executequery(query1);
            u.un1 = GetNewsList(dt1);
            return View(u);
            }
            else { return RedirectToAction("Login", "Login"); }
        }
        public List<uploadnew> GetlistData(DataTable dt)
        {
            List<uploadnew> objupdatedata = new List<uploadnew>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    objupdatedata.Add(new uploadnew { rid = Convert.ToInt32(dr["RegId"]), user = dr["Email"].ToString() });
                }
            }
            return objupdatedata;
        }

        public List<uploadnew> GetNewsList(DataTable dt)
        {
            List<uploadnew> objupdatedata = new List<uploadnew>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string image = "";
                    if (dr["Nimage"] != DBNull.Value)
                    {
                        byte[] arr = (byte[])dr["Nimage"];
                        image = "data:image/jpg;base64," + Convert.ToBase64String(arr);
                    }
                    objupdatedata.Add(new uploadnew { NId = Convert.ToInt32(dr["NId"]), image = image, Videoaudio = dr["NVideo"].ToString(), heading = dr["NHeading"].ToString(), createdDate = dr["NcreatedDate"].ToString() });
                }
            }
            return objupdatedata;
        }
        // code to Remove filename and append the timestamp 
        public static string AppendTimeStamp(string fileName)
        {
            return string.Concat(
                // Path.GetFileNameWithoutExtension(fileName),
                DateTime.Now.ToString("yyyyMMddHHmmssffftt"),
                Path.GetExtension(fileName)
                );
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Admin(uploadnew nl, string btnsubscribe, string submit, string Update)
        { if (Session["adiminRegId"] != null)
            {
            ViewBag.cat = getNewsCat();

            ViewBag.Getallcat = getNewsCatAll();
            string filepath, tsfilename = "";
            if (btnsubscribe == "Subscribe")
            {
                TempData["saveData"] = nl.Savesubscription(nl);
            }
            if (this.ModelState.IsValid)
            {
                byte[] imgarray = new byte[0];
                bool isimage = false;
                bool isvideo = false;
                bool isAudio = false;

                    if (submit == "submit")
                    {
                        if (nl.NLImage != null && nl.NLImage.ContentLength > 0)
                        {
                        isimage = v.IsImage(nl.NLImage);
                        isvideo = v.IsVideo(nl.NLImage);
                        isAudio = v.IsAudio(nl.NLImage);
                        if (isimage || isvideo || isAudio)
                        {
                            
                                var fileName = Path.GetFileName(nl.NLImage.FileName);
                                 tsfilename = AppendTimeStamp(fileName);
                                if (isimage)
                                {
                                    filepath = Path.Combine(Server.MapPath("~/images"), tsfilename);
                                    nl.NLImage.SaveAs(filepath);
                                    imgarray = ImageToByteArray(filepath);
                                }else if(isvideo){
                                    filepath = Path.Combine(Server.MapPath("~/Video"), tsfilename);
                                    nl.NLImage.SaveAs(filepath);
                                }
                                else if (isAudio)
                                {
                                    filepath = Path.Combine(Server.MapPath("~/Audio"), tsfilename);
                                    nl.NLImage.SaveAs(filepath);
                                }
                            }
                        }
                    nl.category = GetCatnameonid(nl.catid);
                    SqlCommand cmd = new SqlCommand("sp_Save_News");
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@NId", SqlDbType.Int).Value = nl.NId;
                    cmd.Parameters.Add("@NHeading", SqlDbType.NVarChar).Value = nl.heading;
                    cmd.Parameters.Add("@NDiscription", SqlDbType.NVarChar).Value = nl.discription;
                    cmd.Parameters.Add("@Nimage", SqlDbType.Image).Value = imgarray;
                    cmd.Parameters.Add("@NVideo", SqlDbType.NVarChar).Value = tsfilename;
                    cmd.Parameters.Add("@NCategory", SqlDbType.NVarChar).Value = nl.category;

                    if (nl.uploadedby != null && nl.uploadedby != "")
                    {
                        cmd.Parameters.Add("@NCreatedby", SqlDbType.NVarChar).Value = nl.uploadedby;
                        cmd.Parameters.Add("@NUpdatedby", SqlDbType.NVarChar).Value = nl.uploadedby;
                    }
                    else
                    {
                        cmd.Parameters.Add("@NCreatedby", SqlDbType.NVarChar).Value = Session["Adminname"].ToString();
                        cmd.Parameters.Add("@NUpdatedby", SqlDbType.NVarChar).Value = Session["Adminname"].ToString();
                    }
                    cmd.Parameters.Add("@NcreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@NUpdatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@NCatId", SqlDbType.Int).Value = nl.catid;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    TempData["saveData"] = "News saved successfully...!!!";
                }
                else if (Update == "Update")
                    {
                        if (nl.NLImage1 != null && nl.NLImage1.ContentLength > 0)
                        {
                            isimage = v.IsImage(nl.NLImage1);
                            isvideo = v.IsVideo(nl.NLImage1);
                            isAudio = v.IsAudio(nl.NLImage1);
                            if (isimage || isvideo || isAudio)
                            {

                                var fileName = Path.GetFileName(nl.NLImage1.FileName);
                                tsfilename = AppendTimeStamp(fileName);
                                if (isimage)
                                {
                                    filepath = Path.Combine(Server.MapPath("~/images"), tsfilename);
                                    nl.NLImage1.SaveAs(filepath);
                                    imgarray = ImageToByteArray(filepath);
                                }
                                else if (isvideo)
                                {
                                    filepath = Path.Combine(Server.MapPath("~/Video"), tsfilename);
                                    nl.NLImage1.SaveAs(filepath);
                                }
                                else if (isAudio)
                                {
                                    filepath = Path.Combine(Server.MapPath("~/Audio"), tsfilename);
                                    nl.NLImage1.SaveAs(filepath);
                                }
                            }
                        }
                    nl.category = GetCatnameonid(nl.catid1);
                    SqlCommand cmd = new SqlCommand("sp_update_News");
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@NId", SqlDbType.Int).Value = nl.uphiddenid;
                    cmd.Parameters.Add("@NHeading", SqlDbType.NVarChar).Value = nl.heading1;
                    cmd.Parameters.Add("@NDiscription", SqlDbType.NVarChar).Value = nl.discription1;
                    cmd.Parameters.Add("@Nimage", SqlDbType.Image).Value = imgarray;
                    if (nl.Videoaudio1 != "" && nl.Videoaudio1 != null && tsfilename == "")
                        cmd.Parameters.Add("@NVideo", SqlDbType.NVarChar).Value = nl.Videoaudio1;
                    else
                    cmd.Parameters.Add("@NVideo", SqlDbType.NVarChar).Value = tsfilename;

                    cmd.Parameters.Add("@NCategory", SqlDbType.NVarChar).Value = nl.category;

                    if (nl.uploadedby != null && nl.uploadedby != "")
                    {
                        cmd.Parameters.Add("@NCreatedby", SqlDbType.NVarChar).Value = nl.uploadedby;
                        cmd.Parameters.Add("@NUpdatedby", SqlDbType.NVarChar).Value = nl.uploadedby;
                    }
                    else
                    {
                        cmd.Parameters.Add("@NCreatedby", SqlDbType.NVarChar).Value = Session["Adminname"].ToString();
                        cmd.Parameters.Add("@NUpdatedby", SqlDbType.NVarChar).Value = Session["Adminname"].ToString();
                    }
                    cmd.Parameters.Add("@NcreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@NUpdatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@NCatId", SqlDbType.Int).Value = nl.catid;
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
               
            }
            return RedirectToAction("Admin");
            }
        else { return RedirectToAction("Login", "Login"); }
        }

        public string GetNews(string NId)
        {
            DataTable dt = new DataTable();
            string str = string.Empty;
            string query1 = "select * from News where NId="+NId;
            dt = Executequery(query1);
            str = GetseriliseResult(dt);
            return str;

        }

        public string GetSubscription(string RegId)
        {
            string str = string.Empty;
            DataTable dt = new DataTable();
            string query = "select * from Client_Subcription where RegId="+RegId;
            str = GetseriliseResult(Executequery(query));
            return str;
        }

        public string GetseriliseResult(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            serializer.MaxJsonLength = 50000000;
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                return serializer.Serialize(rows);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string Savesubscription(string Marathi, string ruralagri, string economical, string English, string GeoPolitical, string Financial, string Management, string Legal, string Sports, string Research, string Education, string Testagainn, string Images, string Videos, string IsNew, string RegId, string Audio)
        {
            if (RegId != "")
            {
                DataTable dt = new DataTable();
                string query = "select * from Client_Subcription where RegId=" + RegId;
                dt = Executequery(query);
                if (dt.Rows.Count > 0)
                {
                   string val=dt.Rows[0]["RegId"].ToString();
                        IsNew = "0";
                }
                else 
                        IsNew="1";
            }

             string str=string.Empty;
             SqlCommand cmd = new SqlCommand("sp_clentsubscription");
             cmd.Connection = con;
             cmd.CommandType = CommandType.StoredProcedure;
             cmd.Parameters.Add("@RegId", SqlDbType.Int).Value =Convert.ToInt32(RegId);
             cmd.Parameters.Add("@Marathi", SqlDbType.Int).Value = Convert.ToInt32(Marathi);
             cmd.Parameters.Add("@English", SqlDbType.Int).Value = Convert.ToInt32(English);
             cmd.Parameters.Add("@Geopolytical", SqlDbType.Int).Value = Convert.ToInt32(GeoPolitical);
             cmd.Parameters.Add("@Finanace", SqlDbType.Int).Value = Convert.ToInt32(Financial);
             cmd.Parameters.Add("@Management", SqlDbType.Int).Value = Convert.ToInt32(Management);
             cmd.Parameters.Add("@legal", SqlDbType.Int).Value = Convert.ToInt32(Legal);
             cmd.Parameters.Add("@Sports", SqlDbType.Int).Value = Convert.ToInt32(Sports);
             cmd.Parameters.Add("@Research", SqlDbType.Int).Value =Convert.ToInt32( Research);
             cmd.Parameters.Add("@Education", SqlDbType.Int).Value = Convert.ToInt32(Education);
             cmd.Parameters.Add("@Testagain", SqlDbType.Int).Value = Convert.ToInt32(Testagainn);
             cmd.Parameters.Add("@images", SqlDbType.Int).Value = Convert.ToInt32(Images);
             cmd.Parameters.Add("@videos", SqlDbType.Int).Value =Convert.ToInt32(Videos);
             cmd.Parameters.Add("@IsNew", SqlDbType.Int).Value = Convert.ToInt32(IsNew);
             cmd.Parameters.Add("@Audio", SqlDbType.Int).Value = Convert.ToInt32(Audio);
             cmd.Parameters.Add("@ruralagri", SqlDbType.Int).Value =Convert.ToInt32(ruralagri);
             cmd.Parameters.Add("@economical", SqlDbType.Int).Value = Convert.ToInt32(economical);
             con.Open();
             cmd.ExecuteNonQuery();
             str = "subscription saved successfully...!!!";
             return str;
        }
        public string DeleteNews(int newsid)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
           
            DataTable dt = new DataTable();
            string str = string.Empty;
            string query1 = "delete from News where NId=" + newsid;
            v.ExecuteNonQuery(query1);
            str = "Record deleted";
            return serializer.Serialize(str);
        }
   
        public string GetCatnameonid(int catid)
        {
            DataTable dt = new DataTable();
            string catname = string.Empty;
            string str = "select NCatId,NCatName,NCatCode,NCatDesc from NewsCategory where NCatId='" + catid + "'";
            dt = Executequery(str);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    catname = dr["NCatName"].ToString();
                }
            }
            else catname = "Homepage";
            return catname;
        }
        public List<SelectListItem> getNewsCat()
        {
            DataTable dt = new DataTable();
            string query = "select NCatId,NCatName,NCatCode,NCatDesc from NewsCategory where NCatHome=1";
            dt = Executequery(query);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    NewsCat.Add(new SelectListItem()
                    {
                        Text = dr["NCatName"].ToString(),
                        Value = dr["NCatId"].ToString()
                    });
                }
            }
            return new List<SelectListItem>(NewsCat);
        }

        public List<SelectListItem> getNewsCatAll()
        {
            DataTable dt = new DataTable();
            string query = "select NCatId,NCatName,NCatCode,NCatDesc from NewsCategory";
            dt = Executequery(query);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    NewsCatAll.Add(new SelectListItem()
                    {
                        Text = dr["NCatName"].ToString(),
                        Value = dr["NCatId"].ToString()
                    });
                }
            }
            return new List<SelectListItem>(NewsCatAll);
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

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        public byte[] ImageToByteArray(string imagefilePath)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(imagefilePath);
            byte[] imageByte = ImageToByteArraybyImageConverter(image);
            return imageByte;
        }
        public byte[] ImageToByteArraybyImageConverter(System.Drawing.Image image)
        {
            ImageConverter imageConverter = new ImageConverter();
            byte[] imageByte = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
            return imageByte;
        }

        public ActionResult Changepassword()
        {
            if (Session["adiminRegId"] != null && Session["Adminname"]!= null)
            {
                ViewBag.name = Session["Adminname"].ToString();
                return View();
            }
            else { return RedirectToAction("Login", "Login"); }
        }
        [HttpPost]
        public ActionResult Changepassword(Changepassword c, string btnsubit)
        {
            if (Session["adiminRegId"] != null && Session["Adminname"] != null)
            {
                if (this.ModelState.IsValid)
                {
                    DataTable dt = new DataTable();
                    string str = string.Empty;
                    str = "select * from tAdmin where Aminid=" + Convert.ToInt32(Session["adiminRegId"]) + " and Apass='" + v.Encryptdata(c.OldPassword) + "'";
                    dt = v.Executequery(str);
                    if (dt.Rows.Count > 0)
                    {
                        DataTable dt1 = new DataTable();
                        string query = "update tAdmin set Apass='" + v.Encryptdata(c.NewPassword) + "',Acpass='" + v.Encryptdata(c.ConfirmPassword) + "' where Aminid=" + Convert.ToInt32(Session["adiminRegId"]) + " and Apass='" + v.Encryptdata(c.OldPassword) + "'";
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