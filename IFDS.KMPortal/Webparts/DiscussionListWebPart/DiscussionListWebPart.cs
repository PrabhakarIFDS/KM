using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;  

namespace IFDS.KMPortal.Webparts.DiscussionListWebPart
{
    [ToolboxItemAttribute(false)]
    public class DiscussionListWebPart : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/IFDS.KMPortal.WebParts/DiscussionListWebPart/DiscussionListWebPartUserControl.ascx";

        protected override void CreateChildControls()
        {
            DiscussionListWebPartUserControl control = (DiscussionListWebPartUserControl)Page.LoadControl(_ascxPath);
            control.sDiscussionErrorMessage = this._discErrMsg;
            control.sDiscussionItemCount  = this._discItemCount ;
            control.sDiscussionListName  = this._discListName ;
            control.sDiscussionSiteURL  = this._discSiteUrl ;
            control.sDiscussionTab1  = this._discTab1 ;
            control.sDiscussionTab2  = this._discTab2;
            control.sDiscussionWidgetTitle = this._discWidgetTitle;
            control.sDiscussionSiteURL  = this.discSiteUrl;
            control.sDiscussionToolTip = this._discToolTip;
            control.bdiscGlobalSite = this.discGlobalSite;
            Controls.Add(control);
        }


        private const string DefaultWidgetTitle = "Discussion Forum";
        public  string discWidgetTitle = DefaultWidgetTitle;
        [Category("Discussion Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Widget Title"),
        WebDescription("Enter Widget Title"),
        DefaultValue(DefaultWidgetTitle)]

        public string _discWidgetTitle
        {
            get { return discWidgetTitle; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter title of the widget");
                discWidgetTitle = value;
            }
        }

        private const string DefaultTab1 = "IFDS";
        public  string discTab1 = DefaultTab1;
        [Category("Discussion Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tab1 Name"),
        WebDescription("Enter name for Tab1"),
        DefaultValue(DefaultTab1)]

        public string _discTab1
        {
            get { return discTab1; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name for tab1");
                discTab1 = value;
            }
        }

        private const string DefaultTab2 = "Department";
        public  string discTab2 = DefaultTab2;
        [Category("Discussion Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tab2 Name"),
        WebDescription("Enter name for Tab2"),
        DefaultValue(DefaultTab2)]

        public string _discTab2
        {
            get { return discTab2; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name for tab2");
                discTab2 = value;
            }
        }

        private const string DefaultListName = "Discussions List";
        public  string discListName = DefaultListName;
        [Category("Discussion Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Discussion List"),
        WebDescription("Enter List Name"),
        DefaultValue(DefaultListName)]

        public string _discListName
        {
            get { return discListName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name of the list");
                discListName = value;
            }
        }

        

        private const string DefaultListCount = "5";
        public  string discItemCount = DefaultListCount;
        [Category("Discussion Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("No of items to display"),
        WebDescription("Enter number of items to display"),
        DefaultValue(DefaultListCount)]

        public string _discItemCount
        {
            get { return discItemCount; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter number of items to display");
                discItemCount = value;
            }
        }

        private const Boolean DefaultGlobalSite = false;
        public Boolean discGlobalSite = DefaultGlobalSite;
        [Category("Discussion Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Global Site"),
        WebDescription("Enter True if it is global site else false for department site"),
        DefaultValue(DefaultGlobalSite)]

        public Boolean _discGlobalSite
        {
            get { return discGlobalSite; }
            set
            {
                discGlobalSite = value;
            }
        }

        public  string discSiteUrl;
        [Category("Discussion Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Site URL"),
        WebDescription("Enter site url ")
        ]
        public string _discSiteUrl
        {
            get { return discSiteUrl; }
            set
            {
                discSiteUrl = value;
            }
        }

        private const string DefaultErrorMsg = "An error occurred while processing : please contact your administrator";
        public  string discErrMsg = DefaultErrorMsg;
        [Category("Discussion Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Error Message"),
        WebDescription("Display Error Message to appear in widget"),
        DefaultValue(DefaultErrorMsg)]

        public string _discErrMsg
        {
            get { return discErrMsg; }
            set
            {
                discErrMsg = value;
            }
        }

        private const string DefaultToolTip = "This widget provides you the information about Discussions";
        public static string discToolTip = DefaultToolTip;
        [Category("Discussion Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tool Tip"),
        WebDescription("Enter Tool Tip "),
         DefaultValue(DefaultToolTip)]
        public string _discToolTip
        {
            get { return discToolTip; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter Tool tip");
                discToolTip = value;
            }
        }




    }





}
