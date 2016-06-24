using IFDS.KMPortal.Helper;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace IFDS.KMPortal.Webparts.BestPracticesWebPart
{
    public partial class BestPracticesWebPartUserControl : UserControl
    {
        public string sBestPracticeWidgetTitle
        {
            get;
            set;
        }
        public string sBestPracticeTab1
        {
            get;
            set;
        }
        public string sBestPracticeTab2
        {
            get;
            set;
        }
        public string sBestPracticeListName
        {
            get;
            set;
        }
        public string sBestPracticeListURL
        {
            get;
            set;
        }
        public string sOnboardingListName
        {
            get;
            set;
        }
        public string sOnboardingListURL
        {
            get;
            set;
        }
        public string sBestPracticeItemCount
        {
            get;
            set;
        }
        public string sBestPracticeErrorMessage
        {
            get;
            set;
        }
        public string sBestPracticeToolTip
        {
            get;
            set;
        }

        public string NoDataMessage = Constants.NO_DATA_ERRMSG;
        public bool UserHavePermissionBest = false;
        public bool UserHavePermissionOnBoarding = false;
        public string WebPartID;

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

                SPList oListBest = oWeb.Lists.TryGetList(sBestPracticeListName);
                if (oListBest != null)
                {
                    bool permission = oListBest.DoesUserHavePermissions(oWeb.CurrentUser, SPBasePermissions.AddListItems);
                    UserHavePermissionBest = permission;
                }

                SPList oListOnBoarding = oWeb.Lists.TryGetList(sOnboardingListName);
                if (oListOnBoarding != null)
                {
                    bool permission = oListOnBoarding.DoesUserHavePermissions(oWeb.CurrentUser, SPBasePermissions.AddListItems);
                    UserHavePermissionOnBoarding = permission;
                }
            }            
            WebPartID = this.ID;
        }

    }
}

