using IFDS.KMPortal.Helper;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace IFDS.KMPortal.Webparts.KMPortalMyFavouriteWebPart
{
    public partial class KMPortalMyFavouriteWebPartUserControl : UserControl
    {
        public string smyfavItemCount
        {
            get;
            set;
        }
        public string smyfavSiteUrl
        {
            get;
            set;
        }
        
        public string smyfavErrMsg
        {
            get;
            set;
        }
        public string smyfavWidgetTitle
        {
            get;
            set;
        }
       
        public string smyfavTooTip
        {
            get;
            set;
        }

        public string smyfavViewAll
        {
            get;
            set;
        }

        public string NoDataMessage = Constants.NO_DATA_ERRMSG;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
       
    }
}
