using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;

namespace IFDS.KMPortal.Webparts.NewsWebpart
{
    [ToolboxItemAttribute(false)]
    public class NewsWebpart : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/IFDS.KMPortal.Webparts/NewsWebpart/NewsWebpartUserControl.ascx";
        protected override void CreateChildControls()
        {
            NewsWebpartUserControl control = (NewsWebpartUserControl)Page.LoadControl(_ascxPath);
            control.sNewsListName = this.NewsListName;
            control.sNewsItemCount = this.NewsItemCount;
            control.sNewsDefaultImageUrl = this.DefaultNewsImage;
            control.sNewsErrMsg = this.NewsErrMsg;
            control.sNewsToolTip = this.NewsToolTip;
            control.sNewsWidgetTitle = this.NewsWidgetTitle;
            control.sNewsSiteUrl = this.NewsSiteUrl;
            Controls.Add(control);

        }


        private const string DefaultWidgetTitle = "News / Events & Announcements";
        public string NewsWidgetTitle = DefaultWidgetTitle;
        [Category("News Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Widget Title"),
        WebDescription("Enter Widget Title"),
        DefaultValue(DefaultWidgetTitle)]
        public string _NewsWidgetTitle
        {
            get { return NewsWidgetTitle; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter title of the widget");
                NewsWidgetTitle = value;
            }
        }


        private const string DefaultNewsListName = "NewsAndAnnouncement";
        public string NewsListName = DefaultNewsListName;
        [Category("News Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("News List"),
        WebDescription("Enter List Name"),
        DefaultValue(DefaultNewsListName)]
        public string _NewsListName
        {
            get { return NewsListName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name of the list");
                NewsListName = value;
            }
        }

        private const string DefaultListCount = "5";
        public string NewsItemCount = DefaultListCount;
        [Category("News Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("No of items to display"),
        WebDescription("Enter number of items to display"),
        DefaultValue(DefaultListCount)]
        public string _NewsItemCount
        {
            get { return NewsItemCount; }
            set
            {
                if (string.IsNullOrEmpty(value) || value == "0")
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter number of items to display");
                NewsItemCount = value;
              }
        }

        
        public string NewsSiteUrl;
        [Category("News Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Site URL"),
        WebDescription("Enter site url ")]
        public string _NewsSiteUrl
        {
            get { return NewsSiteUrl; }
            set
            {               
                NewsSiteUrl = value;
            }
        }


        
        public string DefaultNewsImage;
        [Category("News Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Default Image URL"),
        WebDescription("Enter Image url ")
        ]
        public string _DefaultNewsImage
        {
            get { return DefaultNewsImage; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter Image url");
                DefaultNewsImage = value;
            }
        }

        private const string DefaultErrorMsg = "An error occurred while processing : please contact your administrator";
        public string NewsErrMsg = DefaultErrorMsg;
        [Category("News Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Error Message"),
        WebDescription("Display Error Message to appear in widget"),
        DefaultValue(DefaultErrorMsg)]
        public string _NewsErrMsg
        {
            get { return NewsErrMsg; }
            set
            {
                NewsErrMsg = value;
            }
        }

        private const string DefaultNewsToolTip = "This widget provides you the latest News / Events/ Announcements information related to IFDS";
        public string NewsToolTip = DefaultNewsToolTip;
        [Category("News Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tool Tip"),
        WebDescription("Enter Tool Tip "),
         DefaultValue(DefaultNewsToolTip)]
        public string _NewsToolTip
        {
            get { return NewsToolTip; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter Tool tip");
                NewsToolTip = value;
            }
        }







    }





}
