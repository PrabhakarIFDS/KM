using IFDS.KMPortal.Helper;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace IFDS.KMPortal.Webparts.KMPortalSearchBoxWebPart
{
    public partial class KMPortalSearchBoxWebPartUserControl : UserControl
    {
        public bool bsrchGlobalSite
        {
            get;
            set;
        }
        public string srchRedirect
        {
            get;
            set;
        }
        public string srchLabelText
        {
            get;
            set;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            srchLabelText = (bsrchGlobalSite) ? Constants.GLOBAL_SEARCH : Constants.LOCAL_SEARCH;
        }
    }
}
