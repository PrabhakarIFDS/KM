using IFDS.KMPortal.Helper;
using Microsoft.SharePoint.Client;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace IFDS.KMPortal.Webparts.RecentDocumentsWebPart
{
    [ToolboxItemAttribute(false)]
    public partial class RecentDocumentsWebPart : WebPart
    {
        #region Variables

        public string NoDataMessage = Constants.NO_DATA_ERRMSG;

        #endregion

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        /// <summary>
        /// Constructor method
        /// </summary>
        public RecentDocumentsWebPart()
        {
        }



        #region Web Part Properties

        private const string DefaultWidgetTitle = "Recently Modified Documents";
        public string BestWidgetTitle = DefaultWidgetTitle;
        [Category("Recent Documents Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Widget Title"),
        WebDescription("Enter Widget Title"),
        DefaultValue(DefaultWidgetTitle)]

        public string _BestWidgetTitle
        {
            get { return BestWidgetTitle; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter title of the widget");
                BestWidgetTitle = value;
            }
        }        

        private const string DefaultItemsCount = "5";
        public string itemCount = DefaultItemsCount;
        [Category("Recent Documents Properties"),
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

        //Site URL 
        private const string SiteUrl = "";
        public string siteURL = SiteUrl;
        [Category("Recent Documents Properties"),
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
        [Category("Recent Documents Properties"),
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

        private const string DefaultToolTip = "This widget provides information about Recently modified documents";
        public string bestToolTip = DefaultToolTip;
        [Category("Recent Documents Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tool Tip"),
        WebDescription("Enter Tool Tip "),
         DefaultValue(DefaultToolTip)]
        public string _bestToolTip
        {
            get { return bestToolTip; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter Tool tip");
                bestToolTip = value;
            }
        }


        #endregion


        #region Methods
        /// <summary>
        /// OnInit method of page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }
        
        /// <summary>
        /// Page Load method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #endregion

    }
}
