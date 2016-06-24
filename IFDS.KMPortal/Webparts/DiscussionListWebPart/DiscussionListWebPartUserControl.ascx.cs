using IFDS.KMPortal.Helper;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
namespace IFDS.KMPortal.Webparts.DiscussionListWebPart
{
    public partial class DiscussionListWebPartUserControl : UserControl
    {

       
        /// <summary>
        /// This method executes when the page loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public string sDiscussionWidgetTitle
        {
            get;
            set;
        }
        public string sDiscussionTab1
        {
            get;
            set;
        }
        public string sDiscussionTab2
        {
            get;
            set;
        }
        public string sDiscussionListName
        {
            get;
            set;
        }
        public string sDiscussionItemCount
        {
            get;
            set;
        }
        public string sDiscussionSiteURL
        {
            get;
            set;
        }
        public string sDiscussionErrorMessage
        {
            get;
            set;
        }

        public string sDiscussionToolTip
        {
            get;
            set;
        }

        public string sDiscussionViewAll
        {
            get;
            set;
        }

        public string sDiscussionPost
        {
            get;
            set;
        }
        public bool bdiscGlobalSite
        {
            get;
            set;
        }

        public string NoDataMessage = Constants.NO_DATA_ERRMSG;
        public bool UserHavePermissionDiscussion = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SPWeb oWeb = SPContext.Current.Web;
                SPList oList = oWeb.Lists.TryGetList(sDiscussionListName);
                if (oList != null)
                {
                    bool permission = oList.DoesUserHavePermissions(oWeb.CurrentUser, SPBasePermissions.AddListItems);
                    UserHavePermissionDiscussion = permission;
                }
            }
        }
    }
}
