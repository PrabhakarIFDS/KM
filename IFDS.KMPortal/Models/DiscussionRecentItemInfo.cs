using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFDS.KMPortal.Models
{
   public class DiscussionRecentItemInfo
    {
        public string Title { get; set; }
        public string ReplyCount { get; set; }
        public string Path { get; set; }
        public string Author { get; set; }
        public string FileRef { get; set; }
        public string Created { get; set; }
        public string loggedInUser { get; set; }
        public List<string> groupArray { get; set; }
        public bool isUserSiteAdmin { get; set; }
        public bool isWorkflowAttached { get; set; }
        public bool enableModeration { get; set; }
        public string WorkflowFieldName { get; set; }
        public string DepartmentName { get; set; }
        public SPListItem oItem { get; set; }
        public DateTime ModifiedTime { get; set; }
    }
}
