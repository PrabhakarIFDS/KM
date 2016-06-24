using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace IFDS.KMPortal.Webparts.KMPortalSearchBoxWebPart
{
    [ToolboxItemAttribute(false)]
    public class KMPortalSearchBoxWebPart : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/IFDS.KMPortal.Webparts/KMPortalSearchBoxWebPart/KMPortalSearchBoxWebPartUserControl.ascx";

        protected override void CreateChildControls()
        {
            KMPortalSearchBoxWebPartUserControl control = (KMPortalSearchBoxWebPartUserControl)Page.LoadControl(_ascxPath);
            control.bsrchGlobalSite = this._srchGlobalSite;
            control.srchRedirect = this._srchRedirectUrl;
            Controls.Add(control);
        }

        private const Boolean DefaultGlobalSite = false;
        public Boolean srchGlobalSite = DefaultGlobalSite;
        [Category("Search Box Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Global Site"),
        WebDescription("Enter True if it is global site else false for department site"),
        DefaultValue(DefaultGlobalSite)]

        public Boolean _srchGlobalSite
        {
            get { return srchGlobalSite; }
            set
            {
                srchGlobalSite = value;
            }
        }

        private const string DefaultRedirectURL = "/Pages/SearchResults.aspx";
        public string srchRedirectUrl = DefaultRedirectURL;
        [Category("Search Box Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Redirection URL"),
        WebDescription("Enter redirection url "),
        DefaultValue(DefaultRedirectURL)
        ]
        public string _srchRedirectUrl
        {
            get { return srchRedirectUrl; }
            set
            {
                srchRedirectUrl = value;
            }
        }
    }
}
