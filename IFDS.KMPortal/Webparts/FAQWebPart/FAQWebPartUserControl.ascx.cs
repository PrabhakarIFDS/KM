using IFDS.KMPortal.Helper;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace IFDS.KMPortal.Webparts.FAQWebPart
{
    public partial class FAQWebPartUserControl : UserControl
    {


        public string sfaqListName
        {
            get;
            set;
        }
        public string sfaqItemCount
        {
            get;
            set;
        }
        public string sfaqSiteUrl
        {
            get;
            set;
        }
        public string sfaqTab1
        {
            get;
            set;
        }
        public string sfaqTab2
        {
            get;
            set;
        }
        public string sfaqErrMsg
        {
            get;
            set;
        }
        public string sfaqWidgetTitle
        {
            get;
            set;
        }
        public bool bfaqGlobalSite
        {
            get;
            set;
        }
        public string sfaqTooTip
        {
            get;
            set;
        }

        public string sfaqViewAll
        {
            get;
            set;
        }

        public string sfaqPost
        {
            get;
            set;
        }

        public string NoDataMessage = Constants.NO_DATA_ERRMSG;
        public bool UserHavePermissionFAQ = true;

        /// <summary>
        /// This method executes when the page loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SPWeb oWeb = SPContext.Current.Web;
                SPList oList = oWeb.Lists.TryGetList(sfaqListName);
                if (oList != null)
                {
                    bool permission = oList.DoesUserHavePermissions(oWeb.CurrentUser, SPBasePermissions.AddListItems);
                    UserHavePermissionFAQ = permission;
                }
            }
        }

    }
}
