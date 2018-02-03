using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vruttasanstha.Models
{
    public class UserNews
    {
        public int NID { get; set; }
        public string newsheading { get; set; }
        public int uphiddenid { get; set; }
        public int nid { get; set; }
        public string nheading { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string date { get; set; }
        public string author { get; set; }
        public string regid { get; set; }
        public List<UserNews> unews = new List<UserNews>();
    }
}