using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;

namespace IFDS.KMPortal.Webparts.FAQWebPart
{
    [ToolboxItemAttribute(false)]
    public class FAQWebPart : WebPart
    {

        //testwerwerwer
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath =  @"~/_CONTROLTEMPLATES/15/IFDS.KMPortal.WebParts/FAQWebPart/FAQWebPartUserControl.ascx";

        protected override void CreateChildControls()
        {
            FAQWebPartUserControl control = (FAQWebPartUserControl)Page.LoadControl(_ascxPath);
            control.sfaqErrMsg  = this._faqErrMsg ;
            control.sfaqItemCount  = this.faqItemCount ;
            control.sfaqListName  = this.faqListName ;
            control.sfaqSiteUrl  = this.faqSiteUrl ;
            control.sfaqTab1  = this.faqTab1 ;
            control.sfaqTab2 = this.faqTab2 ;
            control.sfaqWidgetTitle = this.faqWidgetTitle;
            control.bfaqGlobalSite  = this.faqGlobalSite;
            control.sfaqTooTip = this._faqToolTip;
            Controls.Add(control);
        }

        private const string DefaultWidgetTitle = "FAQs";
        public string faqWidgetTitle = DefaultWidgetTitle;
        [Category("FAQ Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Widget Title"),
        WebDescription("Enter Widget Title"),
        DefaultValue(DefaultWidgetTitle)]

        public string _faqWidgetTitle
        {
            get { return faqWidgetTitle; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter title of the widget");
                faqWidgetTitle = value;
            }
        }

        private const string DefaultTab1 = "IFDS";
        public string faqTab1 = DefaultTab1;
        [Category("FAQ Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tab1 Name"),
        WebDescription("Enter name for Tab1"),
        DefaultValue(DefaultTab1)]

        public string _faqTab1
        {
            get { return faqTab1; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name for tab1");
                faqTab1 = value;
            }
        }

        private const string DefaultTab2 = "Department";
        public string faqTab2 = DefaultTab2;
        [Category("FAQ Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tab2 Name"),
        WebDescription("Enter name for Tab2"),
        DefaultValue(DefaultTab2)]

        public string _faqTab2
        {
            get { return faqTab2; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name for tab2");
                faqTab2 = value;
            }
        }

        private const string DefaultFAQListName = "FAQ";
        public  string faqListName = DefaultFAQListName;
        [Category("FAQ Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("FAQ List Name"),
        WebDescription("FAQ List Properties"),
        DefaultValue(DefaultFAQListName)]

        public string _faqListName
        {
            get { return faqListName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name of the list");
                faqListName = value;
            }
        }

        private const string DefaultFAQListCount = "5";
        public  string faqItemCount = DefaultFAQListCount;
        [Category("FAQ Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("No of FAQs to display"),
        WebDescription("Enter number of FAQs to display"),
        DefaultValue(DefaultFAQListCount)]

        public string _faqItemCount
        {
            get { return faqItemCount; }
            set
            {
                if (string.IsNullOrEmpty(value) || value == "0")
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter number of items to display");
                faqItemCount = value;
            }
        }

        private const Boolean DefaultGlobalSite = false;
        public  Boolean faqGlobalSite = DefaultGlobalSite;
        [Category("FAQ Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Global Site"),
        WebDescription("Enter True if it is global site else false for department site"),
        DefaultValue(DefaultGlobalSite)]

        public Boolean _faqGlobalSite
        {
            get { return faqGlobalSite; }
            set
            {
                faqGlobalSite = value;
            }
        }

        public string faqSiteUrl;
        [Category("FAQ Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Site URL"),
        WebDescription("Enter site url ")
        ]
        public string _faqSiteUrl
        {
            get { return faqSiteUrl; }
            set
            {
                faqSiteUrl = value;
            }
        }

        private const string DefaultErrorMsg = "An error occurred while processing : please contact your administrator";
        public  string faqErrMsg = DefaultErrorMsg;
        [Category("FAQ Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Error Message"),
        WebDescription("Display Error Message to appear in widget"),
        DefaultValue(DefaultErrorMsg)]

        public string _faqErrMsg
        {
            get { return faqErrMsg; }
            set
            {
                faqErrMsg = value;
            }
        }

        private const string DefaultToolTip = "This widget provides you the information about FAQs";
        public string faqToolTip = DefaultToolTip;
        [Category("FAQ Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tool Tip"),
        WebDescription("Enter Tool Tip "),
         DefaultValue(DefaultToolTip)]
        public string _faqToolTip
        {
            get { return faqToolTip; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter Tool tip");
                faqToolTip = value;
            }
        }

    }
}
