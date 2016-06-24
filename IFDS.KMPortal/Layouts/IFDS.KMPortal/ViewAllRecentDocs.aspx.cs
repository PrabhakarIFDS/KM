using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace IFDS.KMPortal.Layouts.IFDS.KMPortal
{
    public partial class ViewAllRecentDocs : LayoutsPageBase
    {
        public string SiteURL ;
        //public string ItemCount;
        //public string Libraries;
        public string IsRecent;
        public string CurrentUserID;
        public string ErrMessage;
        public string WebTitle;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.Form["SiteURL"] ))
                {
                    SiteURL = Request.Form["SiteURL"].ToString();
                }
                //if (!string.IsNullOrEmpty(Request.Form["ItemCount"]))
                //{
                //    ItemCount = Request.Form["ItemCount"].ToString();
                //}
                //if (!string.IsNullOrEmpty(Request.Form["Libraries"]))
                //{
                //    Libraries = Request.Form["Libraries"].ToString();
                //}
                if (!string.IsNullOrEmpty(Request.Form["IsRecent"]))
                {
                    IsRecent = Request.Form["IsRecent"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Form["CurrentUserID"]))
                {
                    CurrentUserID = Request.Form["CurrentUserID"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Form["ErrMsg"]))
                {
                    ErrMessage = Request.Form["ErrMsg"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.Form["WebTitle"]))
                {
                    WebTitle = Request.Form["WebTitle"].ToString();
                }
                
                
                
            }
       
        }
        
    }
}
