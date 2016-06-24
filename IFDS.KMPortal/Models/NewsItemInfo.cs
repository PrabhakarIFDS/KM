using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFDS.KMPortal.Models
{
    public class NewsItemInfo
    {
        public string Title { get; set; }
        public string ID { get; set; }
        public string imageURL { get; set; }
        public string itemURL { get; set; }
        public string modifiedDate { get; set; }
        public string Author { get; set; }
        public string FileDirRef { get; set; }
        public DateTime Modified { get; set; }
    }
}
