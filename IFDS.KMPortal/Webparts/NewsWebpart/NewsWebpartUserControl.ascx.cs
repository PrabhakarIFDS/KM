using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using IFDS.KMPortal.Helper;
using System.Web;
using System.Collections.Generic;
using Microsoft.SharePoint;
namespace IFDS.KMPortal.Webparts.NewsWebpart
{
    public partial class NewsWebpartUserControl : UserControl
    {
        public string NoDataMessage = Constants.NO_DATA_ERRMSG;
        public bool UserHavePermission = true;

        public string sNewsListName
        {
            get;
            set;
        }
        public string sNewsItemCount
        {
            get;
            set;
        }
        public string sNewsSiteUrl
        {
            get;
            set;
        }

        public string sNewsErrMsg
        {
            get;
            set;
        }
        public string sNewsWidgetTitle
        {
            get;
            set;
        }

        public string sNewsDefaultImageUrl
        {
            get;
            set;
        }

        public string sNewsToolTip
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SPWeb oWeb = SPContext.Current.Web;
                SPList oList = oWeb.Lists.TryGetList(sNewsListName);
                if (oList != null)
                {
                    bool permission = oList.DoesUserHavePermissions(oWeb.CurrentUser, SPBasePermissions.AddListItems);
                    UserHavePermission = permission;
                }
            }
        }
    }
}
