using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFDS.KMPortal.Models
{
    public class ItemInfo
    {
        public string Title { get; set; }
        public DateTime date { get; set; }
        public string FileLeafRef { get; set; }
        public string FileRef { get; set; }
        public DateTime Modified { get; set; }
        public string modifiedDate { get; set; }
        public string Author { get; set; }
        public string iconURL { get; set; }
        public string itemURL { get; set; }
        public string fileExtension { get; set; }
        public string itemAbsoluteURL { get; set; }
    }
}
