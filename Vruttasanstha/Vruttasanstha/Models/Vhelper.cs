using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Vruttasanstha.Models
{
    public class Vhelper
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Vrutasanastacon"].ConnectionString);
        public string Encryptdata(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }

        public string Decryptdata(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }

        public string SaveAdminRegistration(Adminreg r, byte[] imgarray)
        {
        
            string message = string.Empty;
            SqlCommand cmd = new SqlCommand("sp_Save_Admin");
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Afname", SqlDbType.NVarChar).Value = r.Afname;
            cmd.Parameters.Add("@Alname", SqlDbType.NVarChar).Value = r.Alname;
            cmd.Parameters.Add("@Aemail", SqlDbType.NVarChar).Value = r.Aemail;
            cmd.Parameters.Add("@Acontactno", SqlDbType.NVarChar).Value = r.Acontactno;
            cmd.Parameters.Add("@Aimage", SqlDbType.Image).Value = imgarray;
            cmd.Parameters.Add("@Apass", SqlDbType.NVarChar).Value = Encryptdata(r.Apass);
            cmd.Parameters.Add("@Acpass", SqlDbType.NVarChar).Value =Encryptdata(r.Acpass);
            cmd.Parameters.Add("@Ausername", SqlDbType.NVarChar).Value = r.Ausername;
            cmd.Parameters.Add("@Asuperadmin", SqlDbType.Int).Value = 0;
            con.Open();
            cmd.ExecuteNonQuery();
            message = "register successfully...!!!";
            return message;
        }
        public string AppendTimeStamp(string fileName)
        {
            return string.Concat(
                // Path.GetFileNameWithoutExtension(fileName),
                DateTime.Now.ToString("yyyyMMddHHmmssffftt"),
                Path.GetExtension(fileName)
                );
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
        public void ExecuteNonQuery(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd_Ad = new SqlCommand(query, con);
                con.Open();
                cmd_Ad.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex) { throw ex; }
        }
        public bool Executedata(string query)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd_Ad = new SqlCommand(query, con);
                con.Open();
                cmd_Ad.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex) { throw ex; }
            return true;
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
        public bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" }; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
        public bool IsVideo(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("video"))
            {
                return true;
            }

            string[] formats = new string[] { ".mp4", ".3gp" }; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
        public bool IsAudio(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("audio"))
            {
                return true;
            }

            string[] formats = new string[] { ".mp3" }; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}