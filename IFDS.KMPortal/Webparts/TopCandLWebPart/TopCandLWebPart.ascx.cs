using System;
using System.Net;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Client;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using IFDS.KMPortal.Layouts.IFDS.KMPortal;
using IFDS.KMPortal.Helper;
using Microsoft.SharePoint;

namespace IFDS.KMPortal.Webparts.TopCandLWebPart
{
    public class DepartmentListMaster
    {
        public string Department { get; set; }
        public string siteurl { get; set; }
    }

    [ToolboxItemAttribute(false)]
    public partial class TopCandLWebPart : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TopCandLWebPart()
        {
        }

        public string DepartmentMasterListJSON;
        public string TopContributorsListJSON;
        public string TopLearnersListJSON;

        private const string DefaultWidgetTitle = "IFDS Top Contributors";
        public string TopCandLWidgetTitle = DefaultWidgetTitle;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Widget Title"),
        WebDescription("Enter Widget Title"),
        DefaultValue(DefaultWidgetTitle)]
        public string _TopCandLWidgetTitle
        {
            get { return TopCandLWidgetTitle; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter title of the widget");
                TopCandLWidgetTitle = value;
            }
        }

        private const string DefaultTab1 = "Top Contributors";
        public string TopCandLTab1 = DefaultTab1;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tab1 Name"),
        WebDescription("Enter name for Tab1"),
        DefaultValue(DefaultTab1)]

        public string _TopCandLTab1
        {
            get { return TopCandLTab1; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name for tab1");
                TopCandLTab1 = value;
            }
        }

        private const string DefaultTab2 = "Top Learners";
        public string TopCandLTab2 = DefaultTab2;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tab2 Name"),
        WebDescription("Enter name for Tab2"),
        DefaultValue(DefaultTab2)]

        public string _TopCandLTab2
        {
            get { return TopCandLTab2; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name for tab2");
                TopCandLTab2 = value;
            }
        }

        private const string DefaultTopContributorsListName = "Community Members";
        public string TopContributorsListName = DefaultTopContributorsListName;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Top Contributors List Name"),
        WebDescription("Top Contributors List Properties"),
        DefaultValue(DefaultTopContributorsListName)]

        public string _TopContributorsListName
        {
            get { return TopContributorsListName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name of the list");
                TopContributorsListName = value;
            }
        }

        private const string DefaultTopContributorsAllDeptListName = "TopContributors-Global";
        public string TopContributorsAllDeptListName = DefaultTopContributorsAllDeptListName;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Top Contributors All Department List Name"),
        WebDescription("Top Contributors All Department List Properties"),
        DefaultValue(DefaultTopContributorsAllDeptListName)]

        public string _TopContributorsAllDeptListName
        {
            get { return TopContributorsAllDeptListName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name of the list");
                TopContributorsAllDeptListName = value;
            }
        }

        private const string DefaultTopLearnersListName = "TopLearners";
        public string TopLearnersListName = DefaultTopLearnersListName;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Top Learners List Name"),
        WebDescription("Top Learners List Properties"),
        DefaultValue(DefaultTopContributorsListName)]

        public string _TopLearnersListName
        {
            get { return TopLearnersListName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name of the list");
                TopLearnersListName = value;
            }
        }

        private const string DefaultDepartmentsMasterListName = "KMPortal Department Sites";
        public string DepartmentsMasterListName = DefaultDepartmentsMasterListName;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Departments Master List Name"),
        WebDescription("Departments Master List Properties"),
        DefaultValue(DefaultDepartmentsMasterListName)]

        public string _DepartmentsMasterListName
        {
            get { return DepartmentsMasterListName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter name of the list");
                DepartmentsMasterListName = value;
            }
        }

        private const int DefaultTopContributorsListCount = 5;
        public int TopContributorsItemCount = DefaultTopContributorsListCount;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("No of Top Contributor's to display"),
        WebDescription("Enter number of Top Contributor's to display"),
        DefaultValue(DefaultTopContributorsListCount)]

        public int _TopContributorsItemCount
        {
            get { return TopContributorsItemCount; }
            set
            {
                if (string.IsNullOrEmpty(value.ToString()) || value == 0)
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter number of items to display");
                TopContributorsItemCount = value;
            }
        }

        private const int DefaultTopLearnersListCount = 5;
        public int TopLearnersItemCount = DefaultTopLearnersListCount;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("No of Top Learner's to display"),
        WebDescription("Enter number of Top Learner's to display"),
        DefaultValue(DefaultTopLearnersListCount)]

        public int _TopLearnersItemCount
        {
            get { return TopLearnersItemCount; }
            set
            {
                if (string.IsNullOrEmpty(value.ToString()) || value == 0)
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter number of items to display");
                TopLearnersItemCount = value;
            }
        }

        private const Boolean DefaultGlobalSite = true;
        public Boolean IsTopCandLGlobalSite = DefaultGlobalSite;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Global Site"),
        WebDescription("Enter True if it is global site else false for department site"),
        DefaultValue(DefaultGlobalSite)]

        public Boolean _TopCandLGlobalSite
        {
            get { return IsTopCandLGlobalSite; }
            set
            {
                IsTopCandLGlobalSite = value;
            }
        }

        private const string DefaultTopCandLWebpartSiteUrl = "https://sharepoint-web.gtm.canada01.ifdsnet.int/";
        public string TopCandLWebpartSiteUrl = DefaultTopCandLWebpartSiteUrl;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Top Contributors and Learners Site URL"),
        WebDescription("Enter site url ")
        ]
        public string _TopCandLWebpartSiteUrl
        {
            get { return TopCandLWebpartSiteUrl; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter site url");
                TopCandLWebpartSiteUrl = value;
            }
        }

        private const string DefaultDepartmentMasterListSiteUrl = "https://sharepoint-web.gtm.canada01.ifdsnet.int/";
        public string DepartmentMasterListSiteUrl = DefaultDepartmentMasterListSiteUrl;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Department Master List Site URL"),
        WebDescription("Enter site url ")
        ]
        public string _DepartmentMasterListSiteUrl
        {
            get { return DepartmentMasterListSiteUrl; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Microsoft.SharePoint.WebPartPages.
                        WebPartPageUserException(
                        "Please enter site url");
                DepartmentMasterListSiteUrl = value;
            }
        }

        private const string DefaultTopCandLErrorMsg = "An error occurred while processing : please contact your administrator";
        public string TopCandLErrMsg = DefaultTopCandLErrorMsg;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Error Message"),
        WebDescription("Display Error Message to appear in widget"),
        DefaultValue(DefaultTopCandLErrorMsg)
        ]
        public string _TopCandLErrMsg
        {
            get { return TopCandLErrMsg; }
            set
            {
                TopCandLErrMsg = value;
            }
        }

        private const string DefaultTopCandLToolTipMsg = "Top Contributors and Learners display items from discussion forums";
        public string TopCandLToolTipMsg = DefaultTopCandLToolTipMsg;
        [Category("Top Contributors and Learners Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Tooltip Message"),
        WebDescription("Display Tooltip Message to appear in widget"),
        DefaultValue(DefaultTopCandLToolTipMsg)
        ]
        public string _TopCandLToolTipMsg
        {
            get { return TopCandLToolTipMsg; }
            set
            {
                TopCandLToolTipMsg = value;
            }
        }

        public string NoDataMessage = Constants.NO_DATA_ERRMSG;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        #region SSOM

        public static string GetAllDepartmentsListSOM(string siteurl, string DepartmentMasterListname)
        {
            List<DepartmentListMaster> dplm = new List<DepartmentListMaster>();
            try
            {
                using (SPSite site = new SPSite(siteurl))
                {
                    using (SPWeb oWeb = site.OpenWeb())
                    {
                        SPList oList = oWeb.Lists.TryGetList(DepartmentMasterListname);
                        SPQuery oQuery = new SPQuery();
                        oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Title' Ascending='True' /></OrderBy><ViewFields><FieldRef Name='Title'/><FieldRef Name='SiteURL'/><FieldRef Name='Department Name'/></ViewFields></Query></View>";
                        SPListItemCollection items = oList.GetItems(oQuery);
                        if (items.Count > 0)
                        {
                            foreach (SPListItem eachdepartment in items)
                            {
                                DepartmentListMaster listitem = new DepartmentListMaster();
                                listitem.Department = Convert.ToString(eachdepartment["Title"]);
                                listitem.siteurl = Convert.ToString(eachdepartment["SiteURL"]);
                                dplm.Add(listitem);
                            }
                        }
                    }
                }
                return new JavaScriptSerializer().Serialize(dplm);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, "Top Contributors and Learner webpart", "Error querying external list -" + DepartmentMasterListname.ToString());
                return ex.Message.ToString();
            }
            
        }

        public static string GetDepartmentNameSOM(string siteurl, string DepartmentMasterListname, string currentSiteUrl)
        {
            List<DepartmentListMaster> dplm = new List<DepartmentListMaster>();
            try
            {
                using (SPSite site = new SPSite(siteurl))
                {
                    using (SPWeb oWeb = site.OpenWeb())
                    {
                        SPList oList = oWeb.Lists.TryGetList(DepartmentMasterListname);
                        SPQuery oQuery = new SPQuery();
                        oQuery.ViewXml = "<View><Query><Where><Contains><FieldRef Name='SiteURL' /><Value Type='Text'>" + currentSiteUrl + "</Value></Contains></Where><ViewFields><FieldRef Name='Title'/><FieldRef Name='SiteURL'/><FieldRef Name='Department Name'/></ViewFields></Query></View>";
                        SPListItemCollection items = oList.GetItems(oQuery);
                        if (items.Count > 0)
                        {
                            foreach (SPListItem eachdepartment in items)
                            {
                                DepartmentListMaster listitem = new DepartmentListMaster();
                                listitem.Department = Convert.ToString(eachdepartment["Title"]);
                                listitem.siteurl = Convert.ToString(eachdepartment["SiteURL"]);
                                dplm.Add(listitem);
                            }
                        }
                    }
                }
                return dplm[0].Department;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, "Top Contributors and Learner webpart", "Error querying external list -" + DepartmentMasterListname.ToString());
                return ex.Message.ToString();
            }

        }

        #endregion
       

        public static string GetAllDepartmentsList(string siteurl, string DepartmentMasterListname)
        {
            List<DepartmentListMaster> dplm = new List<DepartmentListMaster>();

            try
            {
                using (ClientContext clientContext = new ClientContext(siteurl))
                {
                    clientContext.Credentials = CredentialCache.DefaultCredentials;
                    Web site = clientContext.Web;
                    List oList = site.Lists.GetByTitle(DepartmentMasterListname);
                    //ViewCollection collView = oList.Views;
                    //View tview = collView.GetByTitle("All Items");
                    //tview.Update();
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Title' Ascending='True' /></OrderBy><ViewFields><FieldRef Name='Title'/><FieldRef Name='SiteURL'/><FieldRef Name='Department Name'/></ViewFields></Query></View>";
                    ListItemCollection collListItem = oList.GetItems(camlQuery);
                    clientContext.Load(collListItem);
                    clientContext.ExecuteQuery();

                    if (collListItem.Count > 0)
                    {
                        foreach (ListItem eachdepartment in collListItem)
                        {
                            DepartmentListMaster listitem = new DepartmentListMaster();
                            listitem.Department = Convert.ToString(eachdepartment["Title"]);
                            listitem.siteurl = Convert.ToString(eachdepartment["SiteURL"]);
                            dplm.Add(listitem);
                        }
                    }

                }
                return new JavaScriptSerializer().Serialize(dplm);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, "Top Contributors and Learner webpart", "Error querying external list -" + DepartmentMasterListname.ToString());
                return ex.Message.ToString();

            }

            //return "[{'Error' : 'true'}]";
        }


        public static string GetDepartmentName(string siteurl, string DepartmentMasterListname, string currentSiteUrl)
        {
            List<DepartmentListMaster> dplm = new List<DepartmentListMaster>();
            try
            {
                using (ClientContext clientContext = new ClientContext(siteurl))
                {
                    List oList = clientContext.Web.Lists.GetByTitle(DepartmentMasterListname);
                    //ViewCollection collView = oList.Views;
                    //View tview = collView.GetByTitle("All Items");
                    //tview.Update();
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<View><Query><Where><Contains><FieldRef Name='SiteURL' /><Value Type='Text'>" + currentSiteUrl + "</Value></Contains></Where><ViewFields><FieldRef Name='Title'/><FieldRef Name='SiteURL'/><FieldRef Name='Department Name'/></ViewFields></Query></View>";
                    ListItemCollection collListItem = oList.GetItems(camlQuery);
                    clientContext.Load(collListItem);
                    ////, items => items.Include(
                    //                         item => item["Title"],
                    //                         item => item["SiteURL"]));
                    clientContext.ExecuteQuery();
                    if (collListItem.Count > 0)
                    {

                        foreach (ListItem eachdepartment in collListItem)
                        {
                            DepartmentListMaster listitem = new DepartmentListMaster();
                            listitem.Department = Convert.ToString(eachdepartment["Title"]);
                            listitem.siteurl = Convert.ToString(eachdepartment["SiteURL"]);
                            dplm.Add(listitem);
                        }
                    }

                }
                return dplm[0].Department;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, "Top Contributors and Learner webpart", "Error querying external list -" + DepartmentMasterListname.ToString());
                return ex.Message.ToString();
            }

        }

        private string _deptname, _topcontributorslistnametoquery, _initialviewallurl;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsTopCandLGlobalSite)
            {
                _deptname = "All Departments";
                _topcontributorslistnametoquery = TopContributorsAllDeptListName;
                _initialviewallurl = DepartmentMasterListSiteUrl + "Lists/" + TopContributorsAllDeptListName + "/AllItems.aspx";
            }
            else
            {
                //_deptname = GetDepartmentName(DepartmentMasterListSiteUrl, DepartmentsMasterListName, TopCandLWebpartSiteUrl);
                _deptname = GetDepartmentNameSOM(DepartmentMasterListSiteUrl, DepartmentsMasterListName, TopCandLWebpartSiteUrl);
                _topcontributorslistnametoquery = TopContributorsListName;
                _initialviewallurl = TopCandLWebpartSiteUrl + "Lists/Members/MembersAllItems.aspx";
            }

            //DepartmentMasterListJSON = GetAllDepartmentsList(DepartmentMasterListSiteUrl, DepartmentsMasterListName);
            //TopContributorsListJSON = IFDS.KMPortal.Layouts.IFDS.KMPortal.WebMethodPage.GetTopContributors(TopCandLWebpartSiteUrl, _topcontributorslistnametoquery, TopContributorsItemCount);
            //TopLearnersListJSON = IFDS.KMPortal.Layouts.IFDS.KMPortal.WebMethodPage.GetTopLearners(DepartmentMasterListSiteUrl, TopLearnersListName, _deptname, TopLearnersItemCount);
            DepartmentMasterListJSON = GetAllDepartmentsListSOM(DepartmentMasterListSiteUrl, DepartmentsMasterListName);
            TopContributorsListJSON = IFDS.KMPortal.Layouts.IFDS.KMPortal.WebMethodHelper.GetTopContributors(TopCandLWebpartSiteUrl, _topcontributorslistnametoquery, TopContributorsItemCount, TopCandLErrMsg);
            TopLearnersListJSON = IFDS.KMPortal.Layouts.IFDS.KMPortal.WebMethodHelper.GetTopLearners(DepartmentMasterListSiteUrl, TopLearnersListName, _deptname, TopLearnersItemCount, TopCandLErrMsg);
        }


    }
}
