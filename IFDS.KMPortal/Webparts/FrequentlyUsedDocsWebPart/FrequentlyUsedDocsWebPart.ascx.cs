using IFDS.KMPortal.Helper;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace IFDS.KMPortal.Webparts.FrequentlyUsedDocsWebPart
{
    [ToolboxItemAttribute(false)]
    public partial class FrequentlyUsedDocsWebPart : WebPart
    {

        #region Web Part Properties

        private const string DefaultWidgetTitle = "Frequently Used Documents";
        public string FrequentlyUsedWidgetTitle = DefaultWidgetTitle;
        [Category("Frequently Used Documents Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Widget Title"),
        WebDescription("Enter Widget Title"),
        DefaultValue(DefaultWidgetTitle)]

        public string _FrequentlyUsedWidgetTitle
        {
            get { return FrequentlyUsedWidgetTitle; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter title of the widget");
                FrequentlyUsedWidgetTitle = value;
            }
        }

        private const string DefaultItemsCount = "5";
        public string itemCount = DefaultItemsCount;
        [Category("Frequently Used Documents Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("No of items to display"),
        WebDescription("Enter number of items to display"),
        DefaultValue(DefaultItemsCount)]

        public string _itemCount
        {
            get { return itemCount; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter number of items to display");
                itemCount = value;
            }
        }

        private const string SiteUrl="";
        public string siteURL = SiteUrl;
        [Category("Frequently Used Documents Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Site URL"),
        WebDescription("Enter Site URL"),
        DefaultValue(SiteUrl)]

        public string _siteURL
        {
            get { return siteURL; }
            set
            {                
                siteURL = value;
            }
        }


        private const string DefaultErrorMsg = "An error occurred while processing : please contact your administrator";
        public string ErrMsg = DefaultErrorMsg;
        [Category("Frequently Used Documents Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Error Message"),
        WebDescription("Display Error Message to appear in widget"),
        DefaultValue(DefaultErrorMsg)]

        public string _ErrMsg
        {
            get { return ErrMsg; }
            set
            {
                ErrMsg = value;
            }
        }

        private const string DefaultToolTip = "This widget provides information about Frequently Used Documents";
        public string webPartToolTip = DefaultToolTip;
        [Category("Frequently Used Documents Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tool Tip"),
        WebDescription("Enter Tool Tip "),
         DefaultValue(DefaultToolTip)]
        public string _webPartToolTip
        {
            get { return webPartToolTip; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter Tool tip");
                webPartToolTip = value;
            }
        }

        #endregion

        public string NoDataMessage = Constants.NO_DATA_ERRMSG;

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public FrequentlyUsedDocsWebPart()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }



        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
