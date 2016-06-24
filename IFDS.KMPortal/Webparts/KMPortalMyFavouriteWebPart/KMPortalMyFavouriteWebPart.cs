using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;

namespace IFDS.KMPortal.Webparts.KMPortalMyFavouriteWebPart
{
    [ToolboxItemAttribute(false)]
    public class KMPortalMyFavouriteWebPart : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/IFDS.KMPortal.Webparts/KMPortalMyFavouriteWebPart/KMPortalMyFavouriteWebPartUserControl.ascx";

        protected override void CreateChildControls()
        {
            KMPortalMyFavouriteWebPartUserControl control = (KMPortalMyFavouriteWebPartUserControl)Page.LoadControl(_ascxPath);
            control.smyfavErrMsg = this.myfavErrMsg;
            control.smyfavItemCount = this.myfavItemCount;
            control.smyfavSiteUrl = this.myfavSiteUrl;
            control.smyfavWidgetTitle = this.myfavWidgetTitle;
            control.smyfavTooTip = this.myfavToolTip;
            Controls.Add(control);
        }


        private const string DefaultWidgetTitle = "My Favourites";
        public string myfavWidgetTitle = DefaultWidgetTitle;
        [Category("My Favourites Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Widget Title"),
        WebDescription("Enter Widget Title"),
        DefaultValue(DefaultWidgetTitle)]

        public string _myfavWidgetTitle
        {
            get { return myfavWidgetTitle; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter title of the widget");
                myfavWidgetTitle = value;
            }
        }
                
        
        private const string DefaultMyFavListCount = "5";
        public string myfavItemCount = DefaultMyFavListCount;
        [Category("My Favourites Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("No of my favourite's to display"),
        WebDescription("Enter number of my favourite's to display"),
        DefaultValue(DefaultMyFavListCount)]

        public string _myfavItemCount
        {
            get { return myfavItemCount; }
            set
            {
                if (string.IsNullOrEmpty(value) || value == "0")
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter number of items to display");
                myfavItemCount = value;
            }
        }


        public string myfavSiteUrl;
        [Category("My Favourites Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Site URL"),
        WebDescription("Enter site url ")
        ]
        public string _myfavSiteUrl
        {
            get { return myfavSiteUrl; }
            set
            {
                myfavSiteUrl = value;
            }
        }

        private const string DefaultErrorMsg = "An error occurred while processing : please contact your administrator";
        public string myfavErrMsg = DefaultErrorMsg;
        [Category("My Favourites Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Error Message"),
        WebDescription("Display Error Message to appear in widget"),
        DefaultValue(DefaultErrorMsg)]

        public string _myfavErrMsg
        {
            get { return myfavErrMsg; }
            set
            {
                myfavErrMsg = value;
            }
        }

        private const string DefaultMyfavToolTip = "This widget provides you the information about My Favourite's";
        public string myfavToolTip = DefaultMyfavToolTip;
        [Category("My Favourites Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tool Tip"),
        WebDescription("Enter Tool Tip "),
         DefaultValue(DefaultMyfavToolTip)]
        public string _myfavToolTip
        {
            get { return myfavToolTip; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter Tool tip");
                myfavToolTip = value;
            }
        }


    }
}
