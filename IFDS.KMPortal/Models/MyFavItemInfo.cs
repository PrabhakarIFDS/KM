using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFDS.KMPortal.Models
{
    public class MyFavItemInfo
    {
        public string Owner { get; set; }
        public Uri ContentUri { get; set; }
        public string ModifiedDate { get; set; }
        public string IconUrl { get; set; }
        public string FileName { get; set; }
        public string DepartmentName { get; set; }
    }
}
