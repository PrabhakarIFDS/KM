using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFDS.KMPortal.Models
{
    public class DiscussionItemInfo
    {
        public string Title { get; set; }
        public string ReplyCount { get; set; }
        public string Path { get; set; }
        public string Author { get; set; }
        public string FileRef { get; set; }
        public string Created { get; set; }
        public string DepartmentName { get; set; }
    }
}
