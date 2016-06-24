using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;

namespace IFDS.KMPortal.Webparts.BestPracticesWebPart
{
    [ToolboxItemAttribute(false)]
    public class BestPracticesWebPart : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/IFDS.KMPortal.Webparts/BestPracticesWebPart/BestPracticesWebPartUserControl.ascx";

        protected override void CreateChildControls()
        {
            //Control control = Page.LoadControl(_ascxPath);

            //Controls.Add(control);
            BestPracticesWebPartUserControl control = (BestPracticesWebPartUserControl)Page.LoadControl(_ascxPath);
            control.sBestPracticeErrorMessage = this._bestErrMsg;
            control.sBestPracticeItemCount = this._bestItemCount;
            control.sBestPracticeListName = this._bestPracticeListName;
            control.sBestPracticeListURL = this._bestPracticeListUrl;
            control.sBestPracticeTab1 = this._bestTab1;
            control.sBestPracticeTab2 = this._bestTab2;
            control.sBestPracticeWidgetTitle = this._BestWidgetTitle;
            control.sOnboardingListName = this._OnboardingListName;
            control.sOnboardingListURL = this._OnboardingListUrl;
            control.sBestPracticeToolTip = this._bestToolTip;
            control.ID = this.ID;
            Controls.Add(control);

            //HttpContext.Current.Session["BestPracticeWebpart"] = dicBestPractice;

        }


        private const string DefaultWidgetTitle = "Best Practices";
        public string BestWidgetTitle = DefaultWidgetTitle;
        [Category("BestPractices Properties"),
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

        private const string DefaultTab1 = "Best Practices";
        public string bestTab1 = DefaultTab1;
        [Category("BestPractices Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tab1 Name"),
        WebDescription("Enter name for Tab1"),
        DefaultValue(DefaultTab1)]

        public string _bestTab1
        {
            get { return bestTab1; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name for tab1");
                bestTab1 = value;
            }
        }

        private const string DefaultTab2 = "Job-Specific Documents";
        public string bestTab2 = DefaultTab2;
        [Category("BestPractices Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tab2 Name"),
        WebDescription("Enter name for Tab2"),
        DefaultValue(DefaultTab2)]

        public string _bestTab2
        {
            get { return bestTab2; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name for tab2");
                bestTab2 = value;
            }
        }

        private const string DefaultbestPracticeListName = "Best Practices";
        public string bestPracticeListName = DefaultbestPracticeListName;
        [Category("BestPractices Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("BestPractice List Name"),
        WebDescription("Enter List Name"),
        DefaultValue(DefaultbestPracticeListName)]

        public string _bestPracticeListName
        {
            get { return bestPracticeListName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name of the list");
                bestPracticeListName = value;
            }
        }

        public string bestPracticeListUrl;
        [Category("BestPractices Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Best Practice Site URL"),
        WebDescription("Enter site url ")
        ]
        public string _bestPracticeListUrl
        {
            get { return bestPracticeListUrl; }
            set
            {               
                bestPracticeListUrl = value;
            }
        }
        private const string DefaultOnboardingListName = "Onboarding Documents";
        public string OnboardingListName = DefaultOnboardingListName;
        [Category("BestPractices Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("OnboardingList Name"),
        WebDescription("Enter List Name"),
        DefaultValue(DefaultOnboardingListName)]

        public string _OnboardingListName
        {
            get { return OnboardingListName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name of the list");
                OnboardingListName = value;
            }
        }

        public string OnboardingListUrl;
        [Category("BestPractices Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Onboarding Site URL"),
        WebDescription("Enter site url ")
        ]
        public string _OnboardingListUrl
        {
            get { return OnboardingListUrl; }
            set
            {               
                OnboardingListUrl = value;
            }
        }
        private const string DefaultBestPracticeListCount = "5";
        public string bestItemCount = DefaultBestPracticeListCount;
        [Category("BestPractices Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("No of items to display"),
        WebDescription("Enter number of items to display"),
        DefaultValue(DefaultBestPracticeListCount)]

        public string _bestItemCount
        {
            get { return bestItemCount; }
            set
            {
                if (string.IsNullOrEmpty(value) || value == "0")
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter number of items to display");
                bestItemCount = value;
            }
        }

        private const string DefaultErrorMsg = "An error occurred while processing : please contact your administrator";
        public string bestErrMsg = DefaultErrorMsg;
        [Category("BestPractices Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Error Message"),
        WebDescription("Display Error Message to appear in widget"),
        DefaultValue(DefaultErrorMsg)]

        public string _bestErrMsg
        {
            get { return bestErrMsg; }
            set
            {
                bestErrMsg = value;
            }
        }

        private const string DefaultBestToolTip = "This widget provides information about Best practices & Onboarding documents";
        public string bestToolTip = DefaultBestToolTip;
        [Category("BestPractices Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tool Tip"),
        WebDescription("Enter Tool Tip "),
         DefaultValue(DefaultBestToolTip)]
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


    }
}
