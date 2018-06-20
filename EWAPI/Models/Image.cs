
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWAPI.Models
{
    public class Images
    {
        public int id { get; set; }
        public string eid { get; set; }
        public string ename { get; set; }
        public string epath { get; set; }
        public string createtime { get; set; }
        public int num { get; set; }
    }
    public class ImagesInfo
    {
        public int id { get; set; }
        public int pid { get; set; }
        public string ip { get; set; }
        public string cookie { get; set; }
        public DateTime createtime { get; set; }
    }
}
