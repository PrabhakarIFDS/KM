using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFDS.KMPortal.Models
{
    public class FAQRecentItemInfo
    {
        public string DefaultDisplayFormUrl { get; set; }
        public string ID { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string Answer { get; set; }
        public string Title { get; set; }
        public string WorkflowFieldName { get; set; }
        public bool isWorkflowAttached { get; set; }
        public bool enableModeration { get; set; }
        public ListItem Item { get; set; }
        public SPListItem oItem { get; set; }
        public string DepartmentName { get; set; }
        public string loggedInUser { get; set; }
        public List<string> groupArray { get; set; }
        public bool isUserSiteAdmin { get; set; }
    }
}
