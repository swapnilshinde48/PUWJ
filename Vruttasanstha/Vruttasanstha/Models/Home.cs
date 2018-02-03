
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vruttasanstha.Models
{
    public class Home
    {
        public string newsid { get; set; }
        public string NewsHeading { get; set; }
        public string Description { get; set; }
        public string NewsImage { get; set; }
        public string Createddate { get; set; }

        public List<Home> home = new List<Home>();
        public List<Home> EconomicalNews = new List<Home>();

        public List<Home> Geopolitical = new List<Home>();
        public string Newstime { get; set; }
        public string heading { get; set; }
        public int GeoNewsid { get; set; }
        public string GeoNewsDesc { get; set; }
    }
}