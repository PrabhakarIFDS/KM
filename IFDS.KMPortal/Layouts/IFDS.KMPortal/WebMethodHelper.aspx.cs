using IFDS.KMPortal.Helper;
using IFDS.KMPortal.Models;
using Microsoft.Office.Server.Search.Query;
using Microsoft.Office.Server.Social;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Web.Services;


namespace IFDS.KMPortal.Layouts.IFDS.KMPortal
{

    public partial class WebMethodHelper : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region Discussion Forum Widget

        #region Global Page - IFDS Tab

        /// <summary>
        /// IFDS Home Page (Global) - IFDS Tab - Get all the recent discussion from Discussion List of global page
        /// </summary>
        /// <param name="propListName"></param>
        /// <param name="propItemCount"></param>
        /// <param name="propSiteUrl"></param>
        /// <param name="propErrorMessage"></param>
        /// <param name="userName"></param>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMostPopularGlobalDiscussions(string propListName, string propItemCount, string propSiteUrl, string propErrorMessage, string userName, string cacheName)
        {
            string discErrMsg = string.Empty;

            string viewAll = string.Empty;
            string post = string.Empty;
            string discitems = string.Empty;
            string Result = string.Empty;
            string discitemsValue = null, viewAllValue = null, postValue = null;

            List<DiscussionItemInfo> _discRecentItemInfo = new List<DiscussionItemInfo>();
            string WorkflowFieldName = string.Empty;
            bool isWorkflowAttached = false;
            bool enableModeration = false;
            bool isUserSiteAdmin = false;
            List<string> userGroupArray = null;

            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    List<DiscussionItemInfo> Cachereturn = (List<DiscussionItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _discRecentItemInfo = Cachereturn; }
                }
                else
                {
                    using (SPSite site = new SPSite(propSiteUrl))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPList oList = oWeb.Lists.TryGetList(propListName);
                            if (oList != null)
                            {
                                SPQuery oQuery = new SPQuery();
                                if (Convert.ToInt32(propItemCount) != 0)
                                {
                                    oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                                }
                                else
                                {
                                    oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                                }
                                SPListItemCollection items = oList.GetItems(oQuery);

                                //Is User Site Admin
                                isUserSiteAdmin = oWeb.CurrentUser.IsSiteAdmin;

                                Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(propSiteUrl, propListName);
                                WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                _discRecentItemInfo = GetAllGlobalRecentDiscussions(items, propItemCount, propErrorMessage, WorkflowFieldName, isWorkflowAttached, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);
                                if (_discRecentItemInfo != null) { CacheHelper.SaveTocache(cacheName, _discRecentItemInfo); }
                            }
                        }
                    }
                }

                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_discRecentItemInfo);

            }
            catch (Exception ex)
            {
                discErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetMostRecentDiscussions"), errMessage);
            }

            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }
            viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            post = "{\"Post\":\"" + postValue + "\"}";
            string errorMessage = "{\"Message\":\"" + discErrMsg + "\"}";
            Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;
        }

        public static List<DiscussionItemInfo> GetAllGlobalRecentDiscussions(SPListItemCollection items, string propDisplayCount, string propErrMsg, string WorkflowFieldName, bool isWorkflowAttached, bool enableModeration, string loggedInUsername, List<string> userGroups, bool IsUserSiteAdmin)
        {
            List<DiscussionItemInfo> _allRecentdiscItems = new List<DiscussionItemInfo>();
            bool addItem = true;
            try
            {
                if (items != null)
                {
                    foreach (SPListItem oItem in items)
                    {
                        if (!string.IsNullOrEmpty(WorkflowFieldName) || enableModeration)
                        {
                            addItem = CommonHelper.IsAddItemToList(isWorkflowAttached, enableModeration, oItem, WorkflowFieldName, IsUserSiteAdmin);
                        }

                        if (addItem)
                        {
                            DiscussionItemInfo discRecentItem = new DiscussionItemInfo();
                            DateTime dt = (DateTime)oItem[Constants.CREATED];
                            string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                            SPUser spUser = CommonHelper.GetSPUser(oItem, Constants.AUTHOR);
                            discRecentItem.Created = formattedDate;
                            discRecentItem.Path = Uri.EscapeUriString(Convert.ToString(oItem[Constants.FILE_REF]));
                            discRecentItem.Title = Convert.ToString(oItem[Constants.TITLE]);
                            discRecentItem.Author = spUser.Name;
                            discRecentItem.ReplyCount = Convert.ToString(oItem[Constants.ITEM_CHILD_COUNT]);
                            _allRecentdiscItems.Add(discRecentItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetAllRecentDiscussions"), errMessage);
            }
            return _allRecentdiscItems;
        }

        #endregion

        #region Global Page - Department Tab

        /// <summary>
        /// IFDS Home Page (Global) - Department Tab - Get all the discussion from KMPortal Department site list of global home page
        /// </summary>
        /// <param name="propListName"></param>
        /// <param name="propItemCount"></param>
        /// <param name="propSiteUrl"></param>
        /// <param name="propErrorMessage"></param>
        /// <param name="userName"></param>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMostRecentGlobalDiscussions(string propListName, string propItemCount, string propSiteUrl, string propErrorMessage, string userName, string cacheName)
        {
            List<DiscussionItemInfo> _discGlobalRecentItemInfo = new List<DiscussionItemInfo>();
            string discErrMsg = string.Empty;
            string discitemsValue = null, viewAllValue = null, postValue = null;
            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    List<DiscussionItemInfo> Cachereturn = (List<DiscussionItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _discGlobalRecentItemInfo = Cachereturn; }
                }
                else
                {
                    using (SPSite site = new SPSite(propSiteUrl))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPList oList = oWeb.Lists.TryGetList(Constants.KMPORTAL_DEPT_SITES);
                            if (oList != null)
                            {
                                SPQuery oQuery = new SPQuery();
                                oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                                SPListItemCollection items = oList.GetItems(oQuery);
                                _discGlobalRecentItemInfo = GetGlobalRecentDiscListItems(items, propErrorMessage, loggedInUsername, propItemCount);
                                if (_discGlobalRecentItemInfo != null) { CacheHelper.SaveTocache(cacheName, _discGlobalRecentItemInfo); }
                            }
                        }
                    }

                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_discGlobalRecentItemInfo);

            }
            catch (Exception ex)
            {
                discErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetMostRecentGlobalDiscussions"), errMessage);
            }
            string discitems;
            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }
            string errorMessage = "{\"Message\":\"" + discErrMsg + "\"}";
            string viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            string post = "{\"Post\":\"" + postValue + "\"}";
            string Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;

        }

        public static List<DiscussionItemInfo> GetGlobalRecentDiscListItems(SPListItemCollection oItems, string propErrMsg, string userName, string propItemCount)
        {
            List<DiscussionItemInfo> _allglobalrecentdiscItems = new List<DiscussionItemInfo>();
            bool isUserSiteAdmin = false;
            try
            {
                List<DiscussionRecentItemInfo> FinalList = new List<DiscussionRecentItemInfo>();
                List<string> userGroupArray = null;
                foreach (SPListItem oItem in oItems)
                {
                    string siteURL = Convert.ToString(oItem[Constants.SITE_URL]);

                    if (Convert.ToString(oItem[Constants.DEPT_NAME]).ToLower() != "all departments")
                    {
                        using (SPSite site = new SPSite(siteURL))
                        {
                            using (SPWeb oWeb = site.OpenWeb())
                            {
                                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                                bool permission = CommonHelper.DoesUserHaveSitePermissions(oWeb, Constants.DISCUSSIONS_LIST);
                                if (permission)
                                {
                                    SPList oList = oWeb.Lists.TryGetList(Constants.DISCUSSIONS_LIST);
                                    SPQuery oQuery = new SPQuery();
                                    if (Convert.ToInt32(propItemCount) != 0)
                                    {
                                        oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                                    }
                                    else
                                    {
                                        oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + Constants.ROW_LIMIT + "</RowLimit></View>";
                                    }
                                    SPListItemCollection items = oList.GetItems(oQuery);
                                    SPGroupCollection groups = oWeb.SiteGroups;

                                    //Is User Site Admin
                                    isUserSiteAdmin = oWeb.CurrentUser.IsSiteAdmin;

                                    //List<ListItem> ResultList = new List<ListItem>();

                                    Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(siteURL, Constants.DISCUSSIONS_LIST);
                                    string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                    bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                    bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                    foreach (SPListItem item in items)
                                    {
                                        DiscussionRecentItemInfo discRecentItem = new DiscussionRecentItemInfo();
                                        DateTime dt = (DateTime)item[Constants.MODIFIED];
                                        string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                                        SPUser spUser = CommonHelper.GetSPUser(item, Constants.AUTHOR);
                                        discRecentItem.Created = formattedDate;
                                        discRecentItem.Path = Uri.EscapeUriString(Convert.ToString(item[Constants.FILE_REF]));
                                        discRecentItem.Title = Convert.ToString(item[Constants.TITLE]);
                                        discRecentItem.Author = spUser.Name;
                                        discRecentItem.ReplyCount = Convert.ToString(item[Constants.ITEM_CHILD_COUNT]);
                                        discRecentItem.isWorkflowAttached = isWorkflowAttached;
                                        discRecentItem.WorkflowFieldName = WorkflowFieldName;
                                        discRecentItem.enableModeration = enableModeration;
                                        discRecentItem.loggedInUser = loggedInUsername;
                                        discRecentItem.groupArray = userGroupArray;
                                        discRecentItem.isUserSiteAdmin = isUserSiteAdmin;
                                        discRecentItem.DepartmentName = item.ParentList.ParentWeb.Title;
                                        discRecentItem.oItem = item;
                                        FinalList.Add(discRecentItem);
                                    }
                                }
                            }
                        }
                    }
                }
                FinalList = FinalList.OrderByDescending(o => o.Created).ToList();
                if (Convert.ToInt32(propItemCount) != 0)
                {
                    FinalList = FinalList.Take(Convert.ToInt32(propItemCount)).ToList();
                }
                else
                {
                    FinalList = FinalList.Take(5).ToList();
                }


                foreach (DiscussionRecentItemInfo oItem in FinalList)
                {
                    bool addItem = true;
                    if (!string.IsNullOrEmpty(oItem.WorkflowFieldName) || oItem.enableModeration)
                    {
                        addItem = CommonHelper.IsAddItemToList(oItem.isWorkflowAttached, oItem.enableModeration, oItem.oItem, oItem.WorkflowFieldName, oItem.isUserSiteAdmin);
                    }

                    if (addItem)
                    {
                        DiscussionItemInfo discRecentItem = new DiscussionItemInfo();

                        discRecentItem.Created = oItem.Created;
                        discRecentItem.Path = oItem.Path;
                        discRecentItem.Title = oItem.Title;
                        discRecentItem.Author = oItem.Author;
                        discRecentItem.ReplyCount = oItem.ReplyCount;
                        discRecentItem.DepartmentName = oItem.DepartmentName;
                        _allglobalrecentdiscItems.Add(discRecentItem);
                    }
                }

            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetGlobalRecentDiscListItems"), errMessage);
            }
            return _allglobalrecentdiscItems;
        }

        #endregion

        #region Department page - Most Popular Tab

        /// <summary>
        /// IFDS Department Page (Department Site) - Most Popular Tab - Get all the discussion from search query. 
        /// </summary>
        /// <param name="propListName"></param>
        /// <param name="propItemCount"></param>
        /// <param name="propSiteUrl"></param>
        /// <param name="propErrorMessage"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMostPopularDiscussions(string propListName, string propItemCount, string propSiteUrl, string propErrorMessage, string userName, string cacheName)
        {
            string discErrMsg = string.Empty;
            string viewAll = string.Empty;
            string post = string.Empty;
            string discitems = string.Empty;
            string Result = string.Empty;
            string discitemsValue = null, viewAllValue = null, postValue = null;

            System.Collections.Generic.List<DiscussionItemInfo> _discItemInfo = new List<DiscussionItemInfo>();
            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    List<DiscussionItemInfo> Cachereturn = (List<DiscussionItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _discItemInfo = Cachereturn; }
                }
                else
                {
                    DataTable resultDataTable;
                    using (SPSite site = new SPSite(propSiteUrl))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            Uri uri = new Uri(oWeb.Url.ToString());
                            string urlPath = System.Web.HttpUtility.UrlPathEncode(uri.GetLeftPart(UriPartial.Authority) + CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                            string actualURL = string.Format(@"path:{0}", urlPath);
                            string webURL = oWeb.Url.ToString();
                            resultDataTable = GetPopularDiscussionsSearchResulsts(site, webURL, actualURL);
                            if (resultDataTable.Rows.Count <= 0)
                            {
                                if (actualURL.Contains("https://"))
                                {
                                    actualURL = actualURL.Replace("https://", "http://");
                                }
                                resultDataTable = GetPopularDiscussionsSearchResulsts(site, webURL, actualURL);
                            }
                            _discItemInfo = GetAllPopularDiscussions(resultDataTable, propItemCount, propErrorMessage);
                            if (_discItemInfo != null) { CacheHelper.SaveTocache(cacheName, _discItemInfo); }
                        }
                    }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_discItemInfo);

            }
            catch (Exception ex)
            {
                discErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetMostPopularDiscussions"), errMessage);
            }
            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }
            viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            post = "{\"Post\":\"" + postValue + "\"}";
            string errorMessage = "{\"Message\":\"" + discErrMsg + "\"}";
            Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;
        }

        public static DataTable GetPopularDiscussionsSearchResulsts(SPSite site, string webURL, string actualURL)
        {
            DataTable resultDataTable = null;
            try
            {
                KeywordQuery keywordQuery = new Microsoft.Office.Server.Search.Query.KeywordQuery(site);
                StringCollection selectProperties = keywordQuery.SelectProperties;
                selectProperties.Add(Constants.CREATED);
                selectProperties.Add(Constants.FILE_EXTENSION);
                selectProperties.Add(Constants.CONTENT_TYPE_ID);
                selectProperties.Add(Constants.TITLE);
                selectProperties.Add(Constants.AUTHOR);
                selectProperties.Add(Constants.PATH);
                selectProperties.Add(Constants.REPLY_COUNT);
                keywordQuery.QueryText = actualURL;
                SearchExecutor searchExecutor = new SearchExecutor();
                ResultTableCollection resultTableCollection = searchExecutor.ExecuteQuery(keywordQuery);
                var resultTables = resultTableCollection.Filter(Constants.ResultTableType, KnownTableTypes.RelevantResults);
                var resultTable = resultTables.FirstOrDefault();
                resultDataTable = resultTable.Table;
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetPopularDiscussionsSearchResulsts"), errMessage);
            }
            return resultDataTable;
        }

        public static List<DiscussionItemInfo> GetAllPopularDiscussions(DataTable items, string propDisplayCount, string propErrMsg)
        {
            List<DiscussionItemInfo> _alldiscItems = new List<DiscussionItemInfo>();

            try
            {
                if (items != null)
                {
                    int count = 0;

                    foreach (DataRow resultRow in items.Rows)
                    {
                        DiscussionItemInfo discItem = new DiscussionItemInfo();

                        if (count < Convert.ToInt32(propDisplayCount))
                        {
                            bool allowOnly = (!string.IsNullOrEmpty(Convert.ToString(resultRow[Constants.CREATED])) && !Convert.ToString(resultRow[Constants.FILE_EXTENSION]).Contains(Constants.ASPX_FILE_EXTENSION) && !string.IsNullOrEmpty(Convert.ToString(resultRow[Constants.CONTENT_TYPE_ID])));
                            if (allowOnly)
                            {
                                DateTime dt = (DateTime)resultRow[Constants.CREATED];
                                string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                                discItem.Created = formattedDate;
                                discItem.Path = Uri.EscapeUriString(Convert.ToString(resultRow[Constants.PATH]));
                                discItem.Title = Convert.ToString(resultRow[Constants.TITLE]);
                                discItem.Author = Convert.ToString(resultRow[Constants.AUTHOR]);
                                discItem.ReplyCount = Convert.ToString(resultRow[Constants.REPLY_COUNT]);
                                _alldiscItems.Add(discItem);
                                count++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetAllPopularDiscussions"), errMessage);
            }
            return _alldiscItems;
        }

        #endregion

        #region  Department Page - Most Recent Tab

        /// <summary>
        /// IFDS Department Page (Department Site) - Most Recent Tab - Get all the recent discussion from current department discussion list.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetMostRecentDiscussions(string propListName, string propItemCount, string propSiteUrl, string propErrorMessage, string userName, string cacheName)
        {
            string discErrMsg = string.Empty;

            string viewAll = string.Empty;
            string post = string.Empty;
            string discitems = string.Empty;
            string Result = string.Empty;
            string discitemsValue = null, viewAllValue = null, postValue = null;

            List<DiscussionItemInfo> _discRecentItemInfo = new List<DiscussionItemInfo>();
            List<string> userGroupArray = null;
            string WorkflowFieldName = string.Empty;
            bool isWorkflowAttached = false;
            bool enableModeration = false;
            bool isUserSiteAdmin = false;

            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    List<DiscussionItemInfo> Cachereturn = (List<DiscussionItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _discRecentItemInfo = Cachereturn; }
                }
                else
                {
                    using (SPSite site = new SPSite(propSiteUrl))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPList oList = oWeb.Lists.TryGetList(propListName);
                            if (oList != null)
                            {
                                SPQuery oQuery = new SPQuery();
                                if (Convert.ToInt32(propItemCount) != 0)
                                {
                                    oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                                }
                                else
                                {
                                    oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                                }
                                SPListItemCollection items = oList.GetItems(oQuery);

                                loggedInUsername = CommonHelper.DecodeUserName(userName);

                                //Is User Site Admin
                                isUserSiteAdmin = oWeb.CurrentUser.IsSiteAdmin;

                                Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(propSiteUrl, propListName);
                                WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                _discRecentItemInfo = GetAllRecentDiscussions(items, propItemCount, propErrorMessage, WorkflowFieldName, isWorkflowAttached, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);
                                if (_discRecentItemInfo != null) { CacheHelper.SaveTocache(cacheName, _discRecentItemInfo); }
                            }
                        }
                    }
                }

                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_discRecentItemInfo);

            }
            catch (Exception ex)
            {
                discErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetMostRecentDiscussions"), errMessage);
            }

            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }
            viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            post = "{\"Post\":\"" + postValue + "\"}";
            string errorMessage = "{\"Message\":\"" + discErrMsg + "\"}";
            Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;
        }

        public static List<DiscussionItemInfo> GetAllRecentDiscussions(SPListItemCollection items, string propDisplayCount, string propErrMsg, string WorkflowFieldName, bool isWorkflowAttached, bool enableModeration, string loggedInUsername, List<string> userGroups, bool IsUserSiteAdmin)
        {
            List<DiscussionItemInfo> _allRecentdiscItems = new List<DiscussionItemInfo>();
            bool addItem = true;
            try
            {
                if (items != null)
                {
                    foreach (SPListItem oItem in items)
                    {
                        if (!string.IsNullOrEmpty(WorkflowFieldName) || enableModeration)
                        {
                            addItem = CommonHelper.IsAddItemToList(isWorkflowAttached, enableModeration, oItem, WorkflowFieldName, IsUserSiteAdmin);
                        }

                        if (addItem)
                        {
                            DiscussionItemInfo discRecentItem = new DiscussionItemInfo();
                            DateTime dt = (DateTime)oItem[Constants.CREATED];
                            string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                            SPUser spUser = CommonHelper.GetSPUser(oItem, Constants.AUTHOR);
                            discRecentItem.Created = formattedDate;
                            discRecentItem.Path = Uri.EscapeUriString(Convert.ToString(oItem[Constants.FILE_REF]));
                            discRecentItem.Title = Convert.ToString(oItem[Constants.TITLE]);
                            discRecentItem.Author = spUser.Name;
                            discRecentItem.ReplyCount = Convert.ToString(oItem[Constants.ITEM_CHILD_COUNT]);
                            _allRecentdiscItems.Add(discRecentItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetAllRecentDiscussions"), errMessage);
            }
            return _allRecentdiscItems;
        }

        #endregion

        #endregion

        #region FAQ Section Widget

        #region Global Page - IFDS Tab

        /// <summary>
        /// Retrieves Most Popular faq's from global site
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetMostPopularGlobalFAQData(string propListName, string propItemCount, string propSiteUrl, string propErrorMessage, string userName, string cacheName)
        {
            string faqErrMsg = string.Empty;
            string discitemsValue = null, viewAllValue = null, postValue = null;
            bool isUserSiteAdmin = false;
            List<FAQItemInfo> _faqGlobalPopularItemInfo = new List<FAQItemInfo>();
            List<string> userGroupArray = null;
            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    List<FAQItemInfo> Cachereturn = (List<FAQItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _faqGlobalPopularItemInfo = Cachereturn; }
                }
                else
                {
                    using (SPSite site = new SPSite(propSiteUrl))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPList oList = oWeb.Lists.TryGetList(propListName);
                            if (oList != null)
                            {
                                SPQuery oQuery = new SPQuery();
                                if (Convert.ToInt32(propItemCount) != 0)
                                {
                                    oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                                }
                                else
                                {
                                    oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                                }
                                SPListItemCollection items = oList.GetItems(oQuery);
                                SPGroupCollection groups = oWeb.SiteGroups;

                                //Is User Site Admin
                                isUserSiteAdmin = oWeb.CurrentUser.IsSiteAdmin;

                                Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(propSiteUrl, propListName);
                                string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                _faqGlobalPopularItemInfo = GetGlobalPopularFAQ(items, propItemCount, propErrorMessage, WorkflowFieldName, isWorkflowAttached, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);
                                if (_faqGlobalPopularItemInfo != null) { CacheHelper.SaveTocache(cacheName, _faqGlobalPopularItemInfo); }
                            }
                        }
                    }

                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_faqGlobalPopularItemInfo);
            }
            catch (Exception ex)
            {
                faqErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetMostPopularGlobalFAQData"), errMessage);
            }
            string discitems;
            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }
            string errorMessage = "{\"Message\":\"" + faqErrMsg + "\"}";
            string viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            string post = "{\"Post\":\"" + postValue + "\"}";
            string Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;

        }

        /// <summary>
        /// This method updates the FAQItemInfo properties by looping through ListItemcollection
        /// </summary>
        /// <param name="items"></param>
        /// <param name="propDisplayCount"></param>
        /// <param name="propErrMsg"></param>
        /// <returns></returns>
        public static List<FAQItemInfo> GetGlobalPopularFAQ(SPListItemCollection items, string propDisplayCount, string propErrMsg, string WorkflowFieldName, bool isWorkflowAttached, bool enableModeration, string loggedInUserName, List<string> groups, bool isUserSiteAdmin)
        {
            List<FAQItemInfo> _allglobalPopularFaqItems = new List<FAQItemInfo>();
            bool addItem = true;
            try
            {
                if (items != null)
                {
                    foreach (SPListItem oItem in items)
                    {
                        if (!string.IsNullOrEmpty(WorkflowFieldName) || enableModeration)
                        {
                            addItem = CommonHelper.IsAddItemToList(isWorkflowAttached, enableModeration, oItem, WorkflowFieldName, isUserSiteAdmin);
                        }

                        if (addItem)
                        {
                            FAQItemInfo faqGlobalPopItem = new FAQItemInfo();
                            string itemURL = string.Format("{0}?ID={1}", oItem.ParentList.DefaultDisplayFormUrl, oItem[Constants.ID]);
                            DateTime dt = (DateTime)oItem[Constants.CREATED];
                            string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                            faqGlobalPopItem.LastModifiedTime = formattedDate;
                            faqGlobalPopItem.FileRef = Uri.EscapeUriString(itemURL);
                            faqGlobalPopItem.Title = Convert.ToString(oItem[Constants.TITLE]);
                            faqGlobalPopItem.Answer = CommonHelper.LimitString(Convert.ToString(oItem[Constants.ANSWER]), Constants.CHAR_MAXLENGTH);
                            _allglobalPopularFaqItems.Add(faqGlobalPopItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetGlobalPopularFAQ"), errMessage);
            }
            return _allglobalPopularFaqItems;
        }

        #endregion

        #region Global Page - Department Tab

        /// <summary>
        /// Retrieves the most recent faq's from KM department sites List (global site)
        /// </summary>
        /// <param name="propListName"></param>
        /// <param name="propItemCount"></param>
        /// <param name="propSiteUrl"></param>
        /// <param name="propErrorMessage"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMostRecentGlobalFAQData(string propListName, string propItemCount, string propSiteUrl, string propErrorMessage, string userName, string cacheName)
        {
            List<FAQItemInfo> _faqGlobalRecentItemInfo = new List<FAQItemInfo>();
            string faqErrMsg = string.Empty;
            string discitemsValue = null, viewAllValue = null, postValue = null;
            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    List<FAQItemInfo> Cachereturn = (List<FAQItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _faqGlobalRecentItemInfo = Cachereturn; }
                }
                else
                {
                    using (SPSite site = new SPSite(propSiteUrl))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPList oList = oWeb.Lists.TryGetList(Constants.KMPORTAL_DEPT_SITES);
                            if (oList != null)
                            {
                                SPQuery oQuery = new SPQuery();
                                oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                                SPListItemCollection items = oList.GetItems(oQuery);
                                _faqGlobalRecentItemInfo = GetGlobalRecentListItems(items, propErrorMessage, userName, propItemCount);
                                if (_faqGlobalRecentItemInfo != null) { CacheHelper.SaveTocache(cacheName, _faqGlobalRecentItemInfo); }
                            }
                        }
                    }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_faqGlobalRecentItemInfo);

            }
            catch (Exception ex)
            {
                faqErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetMostRecentGlobalFAQData"), errMessage);
            }
            string discitems;
            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }
            string errorMessage = "{\"Message\":\"" + faqErrMsg + "\"}";
            string viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            string post = "{\"Post\":\"" + postValue + "\"}";
            string Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;

        }

        /// <summary>
        /// This method updates the FAQItemInfo properties by looping through ListItemcollection
        /// </summary>
        /// <param name="oItems"></param>
        /// <param name="propErrMsg"></param>
        /// <returns></returns>
        public static List<FAQItemInfo> GetGlobalRecentListItems(SPListItemCollection oItems, string propErrMsg, string userName, string propItemCount)
        {
            List<FAQItemInfo> _allglobalrecentfaqItems = new List<FAQItemInfo>();
            List<string> userGroupArray = null;
            bool isUserSiteAdmin = false;
            try
            {
                List<FAQRecentItemInfo> FinalList = new List<FAQRecentItemInfo>();
                foreach (SPListItem oItem in oItems)
                {
                    string siteURL = Convert.ToString(oItem[Constants.SITE_URL]);
                    if (Convert.ToString(oItem[Constants.DEPT_NAME]).ToLower() != "all departments")
                    {
                        using (SPSite site = new SPSite(siteURL))
                        {
                            using (SPWeb oWeb = site.OpenWeb())
                            {
                                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                                bool permission = CommonHelper.DoesUserHaveSitePermissions(oWeb, Constants.FAQLIST);
                                if (permission)
                                {
                                    SPList oList = oWeb.Lists.TryGetList(Constants.FAQLIST);
                                    SPQuery oQuery = new SPQuery();
                                    if (Convert.ToInt32(propItemCount) != 0)
                                    {
                                        oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                                    }
                                    else
                                    {
                                        oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + Constants.ROW_LIMIT + "</RowLimit></View>";
                                    }
                                    SPListItemCollection items = oList.GetItems(oQuery);
                                    SPGroupCollection groups = oWeb.SiteGroups;

                                    //Is User Site Admin
                                    isUserSiteAdmin = oWeb.CurrentUser.IsSiteAdmin;


                                    //List<ListItem> ResultList = new List<ListItem>();

                                    Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(siteURL, Constants.FAQLIST);
                                    string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                    bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                    bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                    foreach (SPListItem item in items)
                                    {

                                        FAQRecentItemInfo ResultList = new FAQRecentItemInfo();

                                        ResultList.DefaultDisplayFormUrl = item.ParentList.DefaultDisplayFormUrl;
                                        ResultList.ID = Convert.ToString(item[Constants.ID]);
                                        ResultList.ModifiedTime = (DateTime)item[Constants.MODIFIED];
                                        ResultList.Answer = Convert.ToString(item[Constants.ANSWER]);
                                        ResultList.Title = Convert.ToString(item[Constants.TITLE]);
                                        ResultList.WorkflowFieldName = WorkflowFieldName;
                                        ResultList.isWorkflowAttached = isWorkflowAttached;
                                        ResultList.enableModeration = enableModeration;
                                        ResultList.oItem = item;
                                        ResultList.DepartmentName = item.ParentList.ParentWeb.Title;
                                        ResultList.loggedInUser = loggedInUsername;
                                        ResultList.groupArray = userGroupArray;
                                        ResultList.isUserSiteAdmin = isUserSiteAdmin;
                                        FinalList.Add(ResultList);
                                    }

                                }
                            }
                        }
                    }
                }
                FinalList = FinalList.OrderByDescending(o => o.ModifiedTime).ToList();
                if (Convert.ToInt32(propItemCount) != 0)
                {
                    FinalList = FinalList.Take(Convert.ToInt32(propItemCount)).ToList();
                }
                else
                {
                    FinalList = FinalList.Take(5).ToList();
                }

                foreach (FAQRecentItemInfo oItem in FinalList)
                {
                    bool addItem = true;
                    if (!string.IsNullOrEmpty(oItem.WorkflowFieldName) || oItem.enableModeration)
                    {
                        addItem = CommonHelper.IsAddItemToList(oItem.isWorkflowAttached, oItem.enableModeration, oItem.oItem, oItem.WorkflowFieldName, oItem.isUserSiteAdmin);
                    }

                    if (addItem)
                    {
                        FAQItemInfo faqglobalrecentItem = new FAQItemInfo();
                        string itemURL = string.Format("{0}?ID={1}", oItem.DefaultDisplayFormUrl, oItem.ID);
                        DateTime dt = oItem.ModifiedTime;
                        string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                        faqglobalrecentItem.LastModifiedTime = formattedDate;
                        faqglobalrecentItem.DepartmentName = oItem.DepartmentName;
                        faqglobalrecentItem.FileRef = Uri.EscapeUriString(itemURL);
                        faqglobalrecentItem.Answer = CommonHelper.LimitString(oItem.Answer, Constants.CHAR_MAXLENGTH);
                        faqglobalrecentItem.Title = oItem.Title;
                        _allglobalrecentfaqItems.Add(faqglobalrecentItem);
                    }
                }

            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetGlobalRecentListItems"), errMessage);
            }
            return _allglobalrecentfaqItems;
        }

        #endregion

        #region Department Page - Most Popular Tab

        /// <summary>
        /// Retrieves the Most Popular faq's by querying the search api from Department sites
        /// </summary>
        /// <param name="propListName"></param>
        /// <param name="propItemCount"></param>
        /// <param name="propSiteUrl"></param>
        /// <param name="propErrorMessage"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMostPopularDepartmentFAQData(string propListName, string propItemCount, string propSiteUrl, string propErrorMessage, string userName, string cacheName)
        {
            string faqErrMsg = string.Empty;
            string discitemsValue = null, viewAllValue = null, postValue = null;
            List<FAQItemInfo> _faqdeptPopularItemInfo = new List<FAQItemInfo>();
            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    List<FAQItemInfo> Cachereturn = (List<FAQItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _faqdeptPopularItemInfo = Cachereturn; }
                }
                else
                {
                    DataTable resultDataTable;
                    using (SPSite site = new SPSite(propSiteUrl))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            Uri uri = new Uri(oWeb.Url.ToString());
                            string urlPath = System.Web.HttpUtility.UrlPathEncode(uri.GetLeftPart(UriPartial.Authority) + CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                            string actualURL = string.Format(@"path:{0}", urlPath);

                            string webURL = oWeb.Url.ToString();
                            resultDataTable = GetDepartmentFAQSearchResulsts(site, webURL, actualURL);

                            if (resultDataTable.Rows.Count <= 0)
                            {
                                if (actualURL.Contains("https://"))
                                {
                                    actualURL = actualURL.Replace("https://", "http://");
                                }
                                resultDataTable = GetDepartmentFAQSearchResulsts(site, webURL, actualURL);
                            }

                            _faqdeptPopularItemInfo = GetAllDeptPopularFAQ(resultDataTable, propItemCount, propErrorMessage);
                            if (_faqdeptPopularItemInfo != null) { CacheHelper.SaveTocache(cacheName, _faqdeptPopularItemInfo); }
                        }
                    }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_faqdeptPopularItemInfo);

            }
            catch (Exception ex)
            {
                faqErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetMostPopularDepartmentFAQData"), errMessage);
            }
            string discitems;
            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }
            string errorMessage = "{\"Message\":\"" + faqErrMsg + "\"}";
            string viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            string post = "{\"Post\":\"" + postValue + "\"}";
            string Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;

        }

        public static DataTable GetDepartmentFAQSearchResulsts(SPSite site, string webURL, string actualURL)
        {
            DataTable resultDataTable = null;
            try
            {
                KeywordQuery keywordQuery = new Microsoft.Office.Server.Search.Query.KeywordQuery(site);
                StringCollection selectProperties = keywordQuery.SelectProperties;
                selectProperties.Add(Constants.LAST_MODIFIED_TIME);
                selectProperties.Add(Constants.CONTENT_TYPE_ID);
                selectProperties.Add(Constants.TITLE);
                selectProperties.Add(Constants.PATH);
                selectProperties.Add(Constants.ANSWER);
                keywordQuery.QueryText = actualURL;
                SearchExecutor searchExecutor = new SearchExecutor();
                ResultTableCollection resultTableCollection = searchExecutor.ExecuteQuery(keywordQuery);
                var resultTables = resultTableCollection.Filter(Constants.ResultTableType, KnownTableTypes.RelevantResults);
                var resultTable = resultTables.FirstOrDefault();
                resultDataTable = resultTable.Table;
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetDepartmentFAQSearchResulsts"), errMessage);
            }
            return resultDataTable;
        }

        /// <summary>
        /// This method updates the FAQItemInfo property  by looping through the ResultTableCollection
        /// </summary>
        /// <param name="items"></param>
        /// <param name="propDisplayCount"></param>
        /// <param name="propErrMsg"></param>
        /// <returns></returns>
        public static List<FAQItemInfo> GetAllDeptPopularFAQ(DataTable items, string propDisplayCount, string propErrMsg)
        {
            List<FAQItemInfo> _alldeptfaqItems = new List<FAQItemInfo>();

            try
            {
                if (items != null)
                {
                    int count = 0;

                    foreach (DataRow resultRow in items.Rows)
                    {
                        FAQItemInfo faqDeptItem = new FAQItemInfo();

                        if (count < Convert.ToInt32(propDisplayCount))
                        {
                            bool allowOnly = (!string.IsNullOrEmpty(Convert.ToString(resultRow[Constants.LAST_MODIFIED_TIME])) && !string.IsNullOrEmpty(Convert.ToString(resultRow[Constants.CONTENT_TYPE_ID])));
                            if (allowOnly)
                            {
                                DateTime dt = (DateTime)resultRow[Constants.LAST_MODIFIED_TIME];
                                string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                                faqDeptItem.LastModifiedTime = formattedDate;
                                faqDeptItem.FileRef = Uri.EscapeUriString(Convert.ToString(resultRow[Constants.PATH]));
                                faqDeptItem.Title = Convert.ToString(resultRow[Constants.TITLE]);
                                //if (resultRow.[Constants.ANSWER] != null)
                                {
                                    faqDeptItem.Answer = CommonHelper.LimitString(Convert.ToString(resultRow[Constants.ANSWER]), Constants.CHAR_MAXLENGTH);
                                }
                                _alldeptfaqItems.Add(faqDeptItem);
                                count++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetAllDeptPopularFAQ"), errMessage);
            }
            return _alldeptfaqItems;
        }

        #endregion

        #region Department Page - Most Recent Tab

        /// <summary>
        /// Retrieves Most Recent faq's from department sites
        /// </summary>
        /// <param name="propListName"></param>
        /// <param name="propItemCount"></param>
        /// <param name="propSiteUrl"></param>
        /// <param name="propErrorMessage"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMostRecentDepartmentFAQData(string propListName, string propItemCount, string propSiteUrl, string propErrorMessage, string userName, string cacheName)
        {
            string faqErrMsg = string.Empty;
            string discitemsValue = null, viewAllValue = null, postValue = null;
            bool isUserSiteAdmin = false;
            List<FAQItemInfo> _faqdeptRecentItemInfo = new List<FAQItemInfo>();
            List<string> userGroupArray = null;
            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    List<FAQItemInfo> Cachereturn = (List<FAQItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _faqdeptRecentItemInfo = Cachereturn; }
                }
                else
                {
                    using (SPSite site = new SPSite(propSiteUrl))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPList oList = oWeb.Lists.TryGetList(propListName);
                            if (oList != null)
                            {
                                SPQuery oQuery = new SPQuery();
                                if (Convert.ToInt32(propItemCount) != 0)
                                {
                                    oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                                }
                                else
                                {
                                    oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit></View>";
                                }
                                SPListItemCollection items = oList.GetItems(oQuery);
                                SPGroupCollection groups = oWeb.SiteGroups;

                                //Is User Site Admin
                                isUserSiteAdmin = oWeb.CurrentUser.IsSiteAdmin;

                                Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(propSiteUrl, propListName);
                                string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                _faqdeptRecentItemInfo = GetAlldeptRecentFAQ(items, propItemCount, propErrorMessage, WorkflowFieldName, isWorkflowAttached, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);
                                if (_faqdeptRecentItemInfo != null) { CacheHelper.SaveTocache(cacheName, _faqdeptRecentItemInfo); }
                            }
                        }
                    }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_faqdeptRecentItemInfo);
            }
            catch (Exception ex)
            {
                faqErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetMostRecentDepartmentFAQData"), errMessage);
            }

            string discitems;
            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }

            string errorMessage = "{\"Message\":\"" + faqErrMsg + "\"}";
            string viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            string post = "{\"Post\":\"" + postValue + "\"}";
            string Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;

        }

        /// <summary>
        /// This method updates the FAQItemInfo by looping through the ListItemCollection
        /// </summary>
        /// <param name="items"></param>
        /// <param name="propDisplayCount"></param>
        /// <param name="propErrMsg"></param>
        /// <returns></returns>
        public static List<FAQItemInfo> GetAlldeptRecentFAQ(SPListItemCollection items, string propDisplayCount, string propErrMsg, string WorkflowFieldName, bool isWorkflowAttached, bool enableModeration, string loggedInUser, List<string> groups, bool IsUserSiteAdmin)
        {
            List<FAQItemInfo> _alldeptRecentFaqItems = new List<FAQItemInfo>();
            bool addItem = true;
            try
            {
                if (items != null)
                {
                    foreach (SPListItem oItem in items)
                    {

                        if (!string.IsNullOrEmpty(WorkflowFieldName) || enableModeration)
                        {
                            addItem = CommonHelper.IsAddItemToList(isWorkflowAttached, enableModeration, oItem, WorkflowFieldName, IsUserSiteAdmin);
                        }

                        if (addItem)
                        {
                            FAQItemInfo faqDeptRecPopItem = new FAQItemInfo();
                            string itemURL = string.Format("{0}?ID={1}", oItem.ParentList.DefaultDisplayFormUrl, oItem[Constants.ID]);
                            DateTime dt = (DateTime)oItem[Constants.MODIFIED];
                            string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                            faqDeptRecPopItem.LastModifiedTime = formattedDate;
                            faqDeptRecPopItem.FileRef = Uri.EscapeUriString(itemURL);
                            faqDeptRecPopItem.Title = Convert.ToString(oItem[Constants.TITLE]);
                            faqDeptRecPopItem.Answer = CommonHelper.LimitString(Convert.ToString(oItem[Constants.ANSWER]), Constants.CHAR_MAXLENGTH);
                            _alldeptRecentFaqItems.Add(faqDeptRecPopItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetAlldeptRecentFAQ"), errMessage);
            }
            return _alldeptRecentFaqItems;
        }

        #endregion

        #endregion

        #region MyFavourite's Section

        /// <summary>
        /// Retrieves the current logged in user's followed documents from mysites
        /// </summary>
        /// <param name="propItemCount"></param>
        /// <param name="propSiteUrl"></param>
        /// <param name="propErrorMessage"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMyFavouritesData(string propItemCount, string propSiteUrl, string propErrorMessage)
        {
            string myfavErr = string.Empty;
            string myfavitems = string.Empty;
            string myfavviewAllLink = string.Empty;
            string myfavitemsValue = null;
            string myfavViewAll = null;
            List<MyFavItemInfo> _myfavItemInfo = new List<MyFavItemInfo>();

            Microsoft.Office.Server.Social.SPSocialActorType contentType = Microsoft.Office.Server.Social.SPSocialActorType.Document;
            try
            {
                using (SPSite site = new SPSite(propSiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPServiceContext serverContext = SPServiceContext.GetContext(web.Site);
                        UserProfileManager profileManager = new UserProfileManager(serverContext);
                        string userString = web.CurrentUser.LoginName.ToString();
                        UserProfileManager userProfileManager = new UserProfileManager(serverContext);
                        if (userProfileManager.UserExists(userString))
                        {
                            Microsoft.Office.Server.UserProfiles.UserProfile userProfile = profileManager.GetUserProfile(userString);
                            if (userProfile != null)
                            {
                                SPSocialFollowingManager manager = new SPSocialFollowingManager(userProfile);
                                SPSocialActorInfo actorInfo = new SPSocialActorInfo();
                                actorInfo.AccountName = web.CurrentUser.LoginName;
                                actorInfo.ActorType = contentType;
                                int followedDocCount = manager.GetFollowedCount(SPSocialActorTypes.Documents);
                                SPSocialActor[] followedDocResult = manager.GetFollowed(SPSocialActorTypes.Documents);
                                _myfavItemInfo = GetAllMyFavouriteListItems(followedDocCount, followedDocResult, propItemCount, propErrorMessage, propSiteUrl);
                            }
                        }
                    };
                };

                myfavitemsValue = new JavaScriptSerializer().Serialize(_myfavItemInfo);
                myfavViewAll = Uri.EscapeUriString(CommonHelper.GetFollowedDocUrl(propSiteUrl));


            }
            catch (Exception ex)
            {
                myfavErr = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetMyFavouritesData"), errMessage);
            }
            if (string.IsNullOrEmpty(myfavitemsValue)) { myfavitems = "{\"Results\":\"" + myfavitemsValue + "\"}"; } else { myfavitems = "{\"Results\":" + myfavitemsValue + "}"; }
            myfavviewAllLink = "{\"View\":\"" + myfavViewAll + "\"}";
            string errorMessage = "{\"Message\":\"" + myfavErr + "\"}";
            string Result = string.Format("[{0},{1},{2}]", myfavitems, errorMessage, myfavviewAllLink);
            return Result;
        }

        /// <summary>
        /// Updated the MyFavItemInfo by looping through the SPSocial Actors
        /// </summary>
        /// <param name="count"></param>
        /// <param name="actors"></param>
        /// <param name="propDisplayCount"></param>
        /// <param name="propErrMsg"></param>
        /// <param name="propSiteURL"></param>
        /// <returns></returns>
        public static List<MyFavItemInfo> GetAllMyFavouriteListItems(int count, SPSocialActor[] actors, string propDisplayCount, string propErrMsg, string propSiteURL)
        {
            List<MyFavItemInfo> _allmyfavItems = new List<MyFavItemInfo>();
            try
            {
                if (count > 0)
                {
                    int itemcount = 0;
                    SPSocialActorType actorType = actors[0].ActorType;
                    foreach (SPSocialActor actor in actors.Reverse<SPSocialActor>())
                    {
                        MyFavItemInfo myfavItem = new MyFavItemInfo();
                        if (itemcount < Convert.ToInt32(propDisplayCount))
                        {
                            string IconURL = string.Format("/_layouts/images/{0}", CommonHelper.LoadIcon(actor.Name, propSiteURL));
                            string[] DocProp = CommonHelper.GetDocListItemProperties(Convert.ToString(actor.ContentUri));
                            DateTime dt = Convert.ToDateTime(DocProp[0]);
                            string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                            myfavItem.ModifiedDate = formattedDate;
                            myfavItem.IconUrl = IconURL;
                            myfavItem.Owner = DocProp[1];
                            myfavItem.DepartmentName = DocProp[2];
                            myfavItem.ContentUri = actor.ContentUri;
                            myfavItem.FileName = CommonHelper.GetFileNameWithoutExtension(actor.Name);
                            _allmyfavItems.Add(myfavItem);
                            itemcount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetAllMyFavouriteListItems"), errMessage);
            }
            return _allmyfavItems;
        }

        #endregion

        #region News Section

        /// <summary>
        /// This method executes to get most recent news items from the News List.
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="listName"></param>
        /// <param name="errMSG"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetRecentNewsItems(string siteURL, string listName, string itemCount, string errMSG, string defaultImageUrl, string cacheName)
        {
            string mostRecentNews = string.Empty;
            string jsonData = string.Empty;
            string viewAll = string.Empty;
            string upload = string.Empty;

            string jsonDataValue = null, viewAllValue = null, postValue = null;

            List<NewsItemInfo> _newsItemInfo = new List<NewsItemInfo>();
            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    Object Cachereturn = (object)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { jsonDataValue = Cachereturn.ToString(); }
                }
                else
                {
                    using (SPSite site = new SPSite(siteURL))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPList oList = oWeb.Lists.TryGetList(listName);
                            SPQuery oQuery = new SPQuery();
                            if (Convert.ToInt32(itemCount) != 0)
                            {
                                oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + itemCount + "</RowLimit></View>";
                            }
                            else
                            {
                                oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                            }
                            SPListItemCollection items = oList.GetItems(oQuery);

                            if (items.Count > 0)
                            {
                                Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(siteURL, listName);
                                string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                _newsItemInfo = GetNewsDocuments(items, siteURL, isWorkflowAttached, WorkflowFieldName, enableModeration, defaultImageUrl);
                            }

                        }
                    }
                    jsonDataValue = new JavaScriptSerializer().Serialize(_newsItemInfo);
                    object jsonDataObject = (object)jsonDataValue;
                    if (jsonDataValue != null) { CacheHelper.SaveTocache(cacheName, jsonDataObject); }
                }

                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(listName, siteURL)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListDetails(listName, siteURL)[1]);
            }
            catch (Exception ex)
            {
                mostRecentNews = errMSG;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetRecentNewsItems"), errMessage);
            }
            if (string.IsNullOrEmpty(jsonDataValue)) { jsonData = "{\"Results\":\"" + jsonDataValue + "\"}"; } else { jsonData = "{\"Results\":" + jsonDataValue + "}"; }
            viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            upload = "{\"Upload\":\"" + postValue + "\"}";
            string errorMessage = "{\"Message\":\"" + mostRecentNews + "\"}";
            string Result = string.Format("[{0},{1},{2},{3}]", jsonData, errorMessage, upload, viewAll);
            return Result;
        }

        /// <summary>
        /// Get the News document collection.
        /// </summary>        
        /// <returns></returns>
        public static List<NewsItemInfo> GetNewsDocuments(SPListItemCollection items, string siteURL, bool isWorkflowAttached, string fieldName, bool enableModeration, string defaultImageUrl)
        {
            List<NewsItemInfo> _ItemInfo = new List<NewsItemInfo>();
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];

                    NewsItemInfo newItem = new NewsItemInfo();
                    SPUser spUser = CommonHelper.GetSPUser(item, Constants.AUTHOR);
                    if (item[Constants.TITLE] != null)
                    {
                        newItem.Title = item[Constants.TITLE].ToString();
                    }
                    newItem.FileDirRef = item[Constants.FILE__DIR_REF].ToString();
                    newItem.ID = item[Constants.ID].ToString();
                    newItem.Author = spUser.Name;
                    newItem.Modified = (DateTime)item[Constants.MODIFIED];

                    if (item[Constants.IMAGE] != null)
                    {
                        SPFieldUrlValue imgfield = new SPFieldUrlValue(Convert.ToString(item[Constants.IMAGE]));

                        if (imgfield != null)
                            newItem.imageURL = imgfield.Url;
                    }
                    else newItem.imageURL = defaultImageUrl;

                    newItem.itemURL = newItem.FileDirRef + "/DispForm.aspx?ID=" + newItem.ID;
                    newItem.modifiedDate = string.Format("{0:dd MMM yyyy}", newItem.Modified);

                    _ItemInfo.Add(newItem);
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetNewsDocuments"), errMessage);
            }
            return _ItemInfo;
        }

        #endregion

        #region TopContributors and Learner's Section

        /// <summary>
        /// Get Most Recent Data from News and Announcement visual webpart
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetTopContributors(string siteurl, string topcontributorslistname, int rowlimit, string propErrorMessage)
        {

            string Result = string.Empty;
            string topContribErrMsg = string.Empty;
            string topcontrib = string.Empty;
            string topcontribValue = null;
            List<TopContributors> tp = new List<TopContributors>();
            List<SPListItem> FinalList = new List<SPListItem>();
            SPUser person;
            try
            {
                using (SPSite site = new SPSite(siteurl))
                {
                    using (SPWeb oWeb = site.OpenWeb())
                    {
                        oWeb.AllowUnsafeUpdates = true;
                        SPList oList = oWeb.Lists.TryGetList(topcontributorslistname);
                        SPQuery oQuery = new SPQuery();

                        if (Convert.ToInt32(rowlimit) != 0)
                        {

                            oQuery.Query = @"<OrderBy><FieldRef Name=""ReputationScore"" Ascending=""False"" /></OrderBy>";
                            oQuery.RowLimit = Convert.ToUInt32(rowlimit);
                        }
                        else
                        {
                            oQuery.Query = @"<OrderBy><FieldRef Name=""ReputationScore"" Ascending=""False"" /></OrderBy>";
                        }

                        SPListItemCollection items = oList.GetItems(oQuery);
                        if (items.Count > 0)
                        {
                            foreach (SPListItem li in items)
                            {
                                TopContributors listitem = new TopContributors();
                                SPUser spUser = CommonHelper.GetSPUser(li, Constants.MEMEBER);
                                listitem.Member = spUser.Name;
                                listitem.NoOfDiscussions = Convert.ToInt32(li[Constants.NUMBER_OF_DISUCSSIONS]);
                                listitem.NoOfReplies = Convert.ToInt32(li[Constants.NUMBER_OF_REPLIES]);
                                listitem.GiftedBadgeText = Convert.ToString(li[Constants.GIFTED_BADGE_TEXT]);
                                listitem.ReputationScore = Convert.ToInt32(li[Constants.REPUTATION_SCORE]);
                                person = spUser;

                                // Retrieve specific properties by using the GetUserProfilePropertiesFor method. 
                                // The returned collection contains only property values.
                                string[] profilePropertyNames = new string[] { Constants.WORK_EMAIL, Constants.WORK_PHONE, Constants.TITLE, Constants.PERSONAL_SPACE };
                                string[] domainname = person.LoginName.Split('|');
                                string userstring = domainname.Length == 2 ? domainname[1] : domainname[0];

                                SPServiceContext serverContext = SPServiceContext.GetContext(oWeb.Site);
                                UserProfileManager profileManager = new UserProfileManager(serverContext);
                                UserProfileManager userProfileManager = new UserProfileManager(serverContext);
                                PeopleManager peopleManager = new PeopleManager();
                                if (userProfileManager.UserExists(person.LoginName.ToString()))
                                {
                                    UserProfile userProfile = profileManager.GetUserProfile(person.LoginName.ToString());
                                    if (userProfile != null)
                                    {
                                        UserProfilePropertiesForUser profilePropertiesForUser = new UserProfilePropertiesForUser(userstring, profilePropertyNames);
                                        profilePropertiesForUser.AccountName = userstring;
                                        IEnumerable<string> profilePropertyValues = peopleManager.GetUserProfilePropertiesFor(profilePropertiesForUser);

                                        //ArrayList returnvalues = ArrayList.Repeat(null, 2);
                                        if (util.IsIneumerableEmptyorNull<string>(profilePropertyValues))
                                        {
                                            ArrayList profilevalues = new ArrayList(5);
                                            foreach (var s in profilePropertyValues) profilevalues.Add(s.ToString());
                                            if (profilevalues.Count > 0)
                                            {
                                                listitem.emailaddress = profilevalues[0].ToString();
                                                listitem.phonenumber = profilevalues[1].ToString();
                                                if (!string.IsNullOrEmpty(userProfile.PersonalUrl.OriginalString))
                                                {
                                                    listitem.personalurl = userProfile.PersonalUrl.OriginalString;
                                                }
                                                else
                                                {
                                                    listitem.personalurl = Constants.DEFAULT_PERSONAL_URL;
                                                }
                                            }
                                        }
                                        tp.Add(listitem);
                                    }
                                }
                                else
                                {
                                    listitem.personalurl = Constants.DEFAULT_PERSONAL_URL;
                                    tp.Add(listitem);
                                }
                            }
                        }
                        oWeb.AllowUnsafeUpdates = false;
                    }
                }

                topcontribValue = new JavaScriptSerializer().Serialize(tp);
            }
            catch (Exception ex)
            {
                topContribErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetTopContributors"), errMessage);
            }

            if (string.IsNullOrEmpty(topcontribValue)) { topcontrib = "{\"Results\":\"" + topcontribValue + "\"}"; } else { topcontrib = "{\"Results\":" + topcontribValue + "}"; }
            string errorMessage = "{\"Message\":\"" + topContribErrMsg + "\"}";
            Result = string.Format("[{0},{1}]", topcontrib, errorMessage);
            return Result;
        }


        /// <summary>
        /// Get Most Recent Data from News and Announcement visual webpart
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetTopLearners(string siteurl, string toplearnerslistname, string departmentname, int rowlimit, string propErrorMessage)
        {

            string Result = string.Empty;
            string topLearnErrMsg = string.Empty;
            string topLearn = string.Empty;
            string topLearnValue = null;
            List<TopLearnersTable> tp = new List<TopLearnersTable>();
            SPUser person;
            try
            {
                using (SPSite site = new SPSite(siteurl))
                {
                    using (SPWeb oWeb = site.OpenWeb())
                    {
                        oWeb.AllowUnsafeUpdates = true;
                        SPList oList = oWeb.Lists.TryGetList(toplearnerslistname);

                        SPQuery oQuery = new SPQuery();
                        if (Convert.ToInt32(rowlimit) != 0)
                        {
                            oQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='web' /><Value Type='Text'>" + departmentname + "</Value></Eq></Where><OrderBy><FieldRef Name='ReputationScore' Ascending='FALSE'/></OrderBy><ViewFields><FieldRef Name='ReputationScore'/><FieldRef Name='Member'/><FieldRef Name='MemberStatus'/><FieldRef Name='web'/><FieldRef Name='Displayname'/><FieldRef Name='Column1'/><FieldRef Name='Column2'/></ViewFields></Query><RowLimit>" + rowlimit + "</RowLimit></View>";
                        }
                        else
                        {
                            oQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='web' /><Value Type='Text'>" + departmentname + "</Value></Eq></Where><OrderBy><FieldRef Name='ReputationScore' Ascending='FALSE'/></OrderBy><ViewFields><FieldRef Name='ReputationScore'/><FieldRef Name='Member'/><FieldRef Name='MemberStatus'/><FieldRef Name='web'/><FieldRef Name='Displayname'/><FieldRef Name='Column1'/><FieldRef Name='Column2'/></ViewFields></Query></View>";
                        }
                        SPListItemCollection items = oList.GetItems(oQuery);

                        if (items.Count > 0)
                        {
                            foreach (SPListItem listitem in items)
                            {
                                TopLearnersTable topltablerow = new TopLearnersTable();
                                topltablerow.Member = Convert.ToString(listitem[Constants.MEMEBER]);
                                topltablerow.MemberStatus = Convert.ToString(listitem[Constants.MEMEBER_STATUS]);
                                topltablerow.displayname = Convert.ToString(listitem[Constants.DISPLAY_NAME]);
                                topltablerow.Department = Convert.ToString(listitem[Constants.WEB]);
                                topltablerow.ReputationScore = Convert.ToInt32(listitem[Constants.REPUTATION_SCORE]);
                                topltablerow.column1 = Convert.ToInt32(listitem[Constants.DISCUSSION_VIEWS]);
                                topltablerow.column2 = Convert.ToInt32(listitem[Constants.FAQ_VIEWS]);

                                if (!string.IsNullOrEmpty(Convert.ToString(listitem[Constants.MEMEBER])))
                                {
                                    string currentUser = Convert.ToString(listitem[Constants.MEMEBER]);
                                    if (currentUser.Contains('|'))
                                    {
                                        person = oWeb.EnsureUser(currentUser.Split('|')[1]);
                                    }
                                    else
                                    {
                                        person = oWeb.EnsureUser(currentUser);
                                    }
                                }
                                else
                                {
                                    person = oWeb.CurrentUser;
                                }
                                // Retrieve specific properties by using the GetUserProfilePropertiesFor method. 
                                // The returned collection contains only property values.
                                string[] profilePropertyNames = new string[] { Constants.WORK_EMAIL, Constants.WORK_PHONE, Constants.TITLE, Constants.PERSONAL_SITE };
                                string[] domainname = person.LoginName.Split('|');
                                string userstring = domainname.Length == 2 ? domainname[1] : domainname[0];


                                SPServiceContext serverContext = SPServiceContext.GetContext(oWeb.Site);
                                UserProfileManager profileManager = new UserProfileManager(serverContext);
                                UserProfileManager userProfileManager = new UserProfileManager(serverContext);
                                PeopleManager peopleManager = new PeopleManager();
                                if (userProfileManager.UserExists(person.LoginName.ToString()))
                                {
                                    UserProfile userProfile = profileManager.GetUserProfile(person.LoginName.ToString());
                                    if (userProfile != null)
                                    {
                                        UserProfilePropertiesForUser profilePropertiesForUser = new UserProfilePropertiesForUser(userstring, profilePropertyNames);
                                        profilePropertiesForUser.AccountName = userstring;
                                        IEnumerable<string> profilePropertyValues = peopleManager.GetUserProfilePropertiesFor(profilePropertiesForUser);

                                        //ArrayList returnvalues = ArrayList.Repeat(null, 2);
                                        if (util.IsIneumerableEmptyorNull<string>(profilePropertyValues))
                                        {
                                            ArrayList profilevalues = new ArrayList(5);
                                            foreach (var s in profilePropertyValues) profilevalues.Add(s.ToString());
                                            if (profilevalues.Count > 0)
                                            {
                                                topltablerow.emailaddress = profilevalues[0].ToString();
                                                topltablerow.phonenumber = profilevalues[1].ToString();
                                                if (!string.IsNullOrEmpty(userProfile.PersonalUrl.OriginalString))
                                                {
                                                    topltablerow.personalurl = userProfile.PersonalUrl.OriginalString;
                                                }
                                                else
                                                {
                                                    topltablerow.personalurl = Constants.DEFAULT_PERSONAL_URL;
                                                }
                                            }
                                        }
                                        tp.Add(topltablerow);
                                    }
                                }
                                else
                                {
                                    topltablerow.personalurl = Constants.DEFAULT_PERSONAL_URL;
                                    tp.Add(topltablerow);
                                }
                            }
                        }
                        oWeb.AllowUnsafeUpdates = false;
                    }
                }
                topLearnValue = new JavaScriptSerializer().Serialize(tp);
            }
            catch (Exception ex)
            {
                topLearnErrMsg = propErrorMessage;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetTopLearners"), errMessage);
                return ex.Message.ToString();
            }

            if (string.IsNullOrEmpty(topLearnValue)) { topLearn = "{\"Results\":\"" + topLearnValue + "\"}"; } else { topLearn = "{\"Results\":" + topLearnValue + "}"; }
            string errorMessage = "{\"Message\":\"" + topLearnErrMsg + "\"}";
            Result = string.Format("[{0},{1}]", topLearn, errorMessage);
            return Result;
        }


        /// <summary>
        /// Get Most Recent Data from News and Announcement visual webpart
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string LogURL(string logurl, string webname, string displayname, string member, string listname)
        {
            if (string.IsNullOrEmpty(logurl))
            {
                //ErrorMessage.Text = "Guid incorrect or could not find property in Property bag, Please check Property bag";
                Logger.WriteLog(Logger.Category.Unexpected, "TaxonomyNavigation Usercontrol", "Term name / Guid not correct");
                return null;
            }
            if (string.IsNullOrEmpty(webname))
            {
                //ErrorMessage.Text = "Guid incorrect or could not find property in Property bag, Please check Property bag";
                Logger.WriteLog(Logger.Category.Unexpected, "TaxonomyNavigation Usercontrol", "Term name / Guid not correct");
                return null;
            }
            if (string.IsNullOrEmpty(displayname))
            {
                //ErrorMessage.Text = "Guid incorrect or could not find property in Property bag, Please check Property bag";
                Logger.WriteLog(Logger.Category.Unexpected, "TaxonomyNavigation Usercontrol", "Term name / Guid not correct");
                return null;
            }
            if (string.IsNullOrEmpty(member))
            {
                //ErrorMessage.Text = "Guid incorrect or could not find property in Property bag, Please check Property bag";
                Logger.WriteLog(Logger.Category.Unexpected, "TaxonomyNavigation Usercontrol", "Term name / Guid not correct");
                return null;
            }
            if (string.IsNullOrEmpty(listname))
            {
                //ErrorMessage.Text = "Guid incorrect or could not find property in Property bag, Please check Property bag";
                Logger.WriteLog(Logger.Category.Unexpected, "TaxonomyNavigation Usercontrol", "Term name / Guid not correct");
                return null;
            }

            SqlConnection conn = GetSQLConnection();
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    conn.Open();
                    SqlCommand insert = new SqlCommand("Insert into [dbo].[KMP_Z_URLAccessEntries](AccessURL, Member, Displayname, web, List) values(@AccessURL, @Member, @Displayname, @Web, @List)", conn);
                    insert.Connection = conn;
                    insert.Parameters.AddWithValue("@AccessURL", logurl);
                    insert.Parameters.AddWithValue("@Member", member);
                    insert.Parameters.AddWithValue("@Displayname", displayname);
                    insert.Parameters.AddWithValue("@web", webname);
                    insert.Parameters.AddWithValue("@List", listname);
                    insert.ExecuteNonQuery();
                });
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                conn.Dispose();
            }

            return "success";
        }

        private static SqlConnection GetSQLConnection()
        {
            //Add below line in the appSettings section of web.config
            //<add key="IFDSConnection" value="Integrated Security=SSPI;Persist Security Info=True;server=CAD2CA1VSPW03;Initial Catalog=ExternalSystems" />            

            SqlConnection con = null;
            try
            {
                string connectionString = string.Empty;
                if (WebConfigurationManager.AppSettings[Constants.IFDSdbConnection] != null)
                {
                    connectionString = WebConfigurationManager.AppSettings[Constants.IFDSdbConnection];
                }
                con = new SqlConnection(connectionString);

            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetSQLConnection"), errMessage);
            }
            return con;
        }

        #endregion

        #region Recent Best Practice Documents

        /// <summary>
        /// This method used to get most recent items from the best practice tab
        /// </summary>        
        /// <returns></returns>
        [WebMethod]
        public static string GetRecentBestDocuments(string siteURL, string propListName, string propDisplayCount, string userName, string errMSG, string cacheName)
        {
            string recentPracticeDocuments = string.Empty;
            List<ItemInfo> _sortedBestPracticeItemInfo = new List<ItemInfo>();
            string jsonData = string.Empty, viewAll = string.Empty, upload = string.Empty, isBestPractice = string.Empty, Result = string.Empty;
            string jsonDataValue = null, viewAllValue = null, uploadValue = null;
            string errorMessage = string.Empty;
            bool isUserSiteAdmin = false;
            List<string> userGroupArray = null;
            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    Object Cachereturn = (object)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { jsonDataValue = Cachereturn.ToString(); }
                }
                else
                {
                    using (SPSite site = new SPSite(siteURL))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPList oList = oWeb.Lists.TryGetList(propListName);

                            SPQuery oQuery = new SPQuery();
                            if (Convert.ToInt32(propDisplayCount) != 0)
                            {
                                oQuery.ViewXml = "<View Scope=\"Recursive\"><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propDisplayCount + "</RowLimit></View>";
                            }
                            else
                            {
                                oQuery.ViewXml = "<View Scope=\"Recursive\"><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                            }
                            SPListItemCollection items = oList.GetItems(oQuery);

                            if (items.Count > 0)
                            {
                                //Is User Site Admin
                                isUserSiteAdmin = oWeb.CurrentUser.IsSiteAdmin;

                                Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(siteURL, propListName);
                                string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                List<ItemInfo> _bestPracticeItemInfo = GetDocuments(items, siteURL, isWorkflowAttached, WorkflowFieldName, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);

                                foreach (ItemInfo item in _bestPracticeItemInfo)
                                {
                                    ItemInfo newItem = GetOtherItemDetails(item, siteURL);
                                    _sortedBestPracticeItemInfo.Add(newItem);
                                }
                            }
                        }
                    }

                    jsonDataValue = new JavaScriptSerializer().Serialize(_sortedBestPracticeItemInfo);
                    object jsonDataObject = (object)jsonDataValue;
                    if (jsonDataValue != null) { CacheHelper.SaveTocache(cacheName, jsonDataObject); }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, siteURL)[0]);
                uploadValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, siteURL)[2]);
            }
            catch (Exception ex)
            {
                recentPracticeDocuments = errMSG;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetRecentBestDocuments"), errMessage);
            }
            if (string.IsNullOrEmpty(jsonDataValue)) { jsonData = "{\"Results\":\"" + jsonDataValue + "\"}"; } else { jsonData = "{\"Results\":" + jsonDataValue + "}"; }
            viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            upload = "{\"Upload\":\"" + uploadValue + "\"}";
            isBestPractice = "{\"IsBestPractice\":\"1\"}";
            errorMessage = "{\"Message\":\"" + recentPracticeDocuments + "\"}";
            Result = string.Format("[{0},{1},{2},{3},{4}]", jsonData, errorMessage, upload, viewAll, isBestPractice);
            return Result;
        }

        /// <summary>
        /// Get the Document item details
        /// </summary>        
        /// <returns></returns>
        public static List<ItemInfo> GetDocuments(SPListItemCollection items, string siteURL, bool isWorkflowAttached, string WorkflowFieldName, bool enableModeration, string loggedInUsername, List<string> userGroups, bool isUserSiteAdmin)
        {
            List<ItemInfo> _ItemInfo = new List<ItemInfo>();
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    ItemInfo newItem = null;
                    bool addItem = true;
                    if (isWorkflowAttached || enableModeration)
                    {
                        addItem = CommonHelper.IsAddItemToList(isWorkflowAttached, enableModeration, item, WorkflowFieldName, isUserSiteAdmin);
                    }
                    if (addItem)
                    {
                        newItem = GetDocumentItem(item, siteURL);
                    }

                    if (newItem != null)
                    {
                        _ItemInfo.Add(newItem);
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetDocuments"), errMessage);
            }
            return _ItemInfo;
        }

        /// <summary>
        /// Get the document item properties.
        /// </summary>       
        /// <returns></returns>
        public static ItemInfo GetDocumentItem(SPListItem item, string siteURL)
        {
            ItemInfo newItem = new ItemInfo();
            try
            {
                SPUser spUser = CommonHelper.GetSPUser(item, Constants.AUTHOR);

                if (item[Constants.FILE__LEAF_REF] != null)
                {
                    newItem.FileLeafRef = item[Constants.FILE__LEAF_REF].ToString();
                }
                if (item[Constants.FILE_REF] != null)
                {
                    newItem.FileRef = item[Constants.FILE_REF].ToString();
                }

                newItem.Author = spUser.Name;

                if (item[Constants.MODIFIED] != null)
                {
                    newItem.Modified = (DateTime)item[Constants.MODIFIED];
                }
                if (item[Constants.OFFICE_EXT_TYPE] != null)
                {
                    newItem.fileExtension = item[Constants.OFFICE_EXT_TYPE].ToString();
                }

                if (item.Fields.ContainsField(Constants.TITLE))
                {
                    if (item[Constants.TITLE] != null)
                    {
                        newItem.Title = item[Constants.TITLE].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = item.File.ServerRelativeUrl + " " + ex.Message + " " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetDocumentItem"), errMessage);
            }
            return newItem;
        }


        /// <summary>
        /// Get the item properties of the list
        /// </summary>
        /// <returns></returns>
        public static ItemInfo GetOtherItemDetails(ItemInfo newItem, string siteURL)
        {
            try
            {
                newItem.iconURL = CommonHelper.LoadRecentIcon(siteURL, newItem.FileLeafRef);
                siteURL = (!siteURL.EndsWith("/")) ? siteURL + "/" : siteURL;
                string docEditURL = siteURL + "_layouts/15/WopiFrame.aspx?sourcedoc=" + newItem.FileRef + "&action=default";
                string itemURL = newItem.FileRef;
                string itemFullUrl = SPUtility.GetFullUrl(SPContext.Current.Site, itemURL);
                newItem.itemAbsoluteURL = itemFullUrl;
                newItem.modifiedDate = string.Format("{0:dd MMM yyyy}", newItem.Modified);
                if (newItem.fileExtension != null)
                {
                    bool officeDoc = Constants.OFFICE_EXT.Any(newItem.fileExtension.Contains);
                    newItem.itemURL = officeDoc ? docEditURL : itemFullUrl;
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetOtherItemDetails"), errMessage);
            }
            return newItem;
        }

        #endregion

        #region Recent OnBoarding Documents

        /// <summary>
        /// This method used to get most recent items from the  onboarding Documents tab
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="propListName"></param>
        /// <param name="errMSG"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetRecentOnboardingDocuments(string siteURL, string propListName, string propDisplayCount, string userName, string errMSG, string cacheName)
        {
            string recentOnboarding = string.Empty;
            string jsonData = string.Empty;
            string viewAll = string.Empty;
            string upload = string.Empty;
            string isBestPractice = string.Empty;
            string jsonDataValue = null, viewAllValue = null, uploadValue = null;
            List<ItemInfo> _sortedOnBoardingItemInfo = new List<ItemInfo>();
            bool isUserSiteAdmin = false;
            List<string> userGroupArray = null;
            try
            {

                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    Object Cachereturn = (object)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { jsonDataValue = Cachereturn.ToString(); }
                }
                else
                {
                    using (SPSite site = new SPSite(siteURL))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPList oList = oWeb.Lists.TryGetList(propListName);

                            SPQuery oQuery = new SPQuery();
                            if (Convert.ToInt32(propDisplayCount) != 0)
                            {
                                oQuery.ViewXml = "<View Scope=\"Recursive\"><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propDisplayCount + "</RowLimit></View>";
                            }
                            else
                            {
                                oQuery.ViewXml = "<View Scope=\"Recursive\"><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                            }
                            SPListItemCollection items = oList.GetItems(oQuery);

                            if (items.Count > 0)
                            {
                                //Is User Site Admin
                                isUserSiteAdmin = oWeb.CurrentUser.IsSiteAdmin;

                                Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(siteURL, propListName);
                                string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                List<ItemInfo> _onBoardingItemInfo = GetDocuments(items, siteURL, isWorkflowAttached, WorkflowFieldName, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);

                                foreach (ItemInfo item in _onBoardingItemInfo)
                                {
                                    ItemInfo newItem = GetOtherItemDetails(item, siteURL);
                                    _sortedOnBoardingItemInfo.Add(newItem);
                                }
                            }
                        }
                    }

                    jsonDataValue = new JavaScriptSerializer().Serialize(_sortedOnBoardingItemInfo);
                    object jsonDataObject = (object)jsonDataValue;
                    if (jsonDataValue != null) { CacheHelper.SaveTocache(cacheName, jsonDataObject); }
                }

                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, siteURL)[0]);
                uploadValue = Uri.EscapeUriString(CommonHelper.GetListDetails(propListName, siteURL)[2]);
            }
            catch (Exception ex)
            {
                recentOnboarding = errMSG;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetRecentOnboardingDocuments"), errMessage);
            }
            if (string.IsNullOrEmpty(jsonDataValue)) { jsonData = "{\"Results\":\"" + jsonDataValue + "\"}"; } else { jsonData = "{\"Results\":" + jsonDataValue + "}"; }
            viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            upload = "{\"Upload\":\"" + uploadValue + "\"}";
            isBestPractice = "{\"IsBestPractice\":\"0\"}";
            string errorMessage = "{\"Message\":\"" + recentOnboarding + "\"}";
            string Result = string.Format("[{0},{1},{2},{3},{4}]", jsonData, errorMessage, upload, viewAll, isBestPractice);
            return Result;
        }

        #endregion

        #region Recently Modified Documents

        /// <summary>
        /// Get Most Recent modified documents from site.
        /// </summary>
        /// <param name="siteURL">Url of Site</param>
        /// <param name="itemCount">Number of items to be fetched</param>
        /// <param name="Libraries">List of libraries to loop</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetRecentlyModifiedDocuments(string siteURL, int itemCount, string userName, string cacheName, string errMSG)
        {
            List<ItemInfo> _allItemInfo = new List<ItemInfo>();
            List<ItemInfo> _sortedItemInfo = new List<ItemInfo>();
            List<ItemInfo> _filteredItemInfo = new List<ItemInfo>();
            string jsonData = null, jsonDataValue = null, recentModifiedError = string.Empty;
            bool isUserSiteAdmin = false;
            List<string> userGroupArray = null;
            try
            {
                string loggedInUsername = SPContext.Current.Web.CurrentUser.LoginName;
                cacheName = loggedInUsername + cacheName;

                if (CacheHelper.IsIncache(cacheName))
                {
                    Object Cachereturn = (object)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { jsonDataValue = Cachereturn.ToString(); }
                }
                else
                {
                    using (SPSite site = new SPSite(siteURL))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPListCollection lists = oWeb.Lists;

                            //Is User Site Admin
                            isUserSiteAdmin = oWeb.CurrentUser.IsSiteAdmin;

                            SPListItemCollection items = null;
                            string[] listArray = Constants.LibrariesToExclude.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                            #region Loop all the lists
                            foreach (SPList list in lists)
                            {
                                string listName = list.Title;

                                if (!list.Hidden && list.BaseType == SPBaseType.DocumentLibrary)
                                {
                                    if (!listArray.Contains(listName))
                                    {
                                        //Query the list for recent documents.
                                        items = GetRecentDocumentsFromCAMLQuery(siteURL, list.Title, itemCount, "Error message");

                                        if (items.Count > 0)
                                        {
                                            //Get Workflow details 
                                            Dictionary<string, string> listDetails = CommonHelper.GetWorkflowDetails(siteURL, listName);
                                            string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                            bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                            bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                            List<ItemInfo> _listItemsInfo = GetDocuments(items, siteURL, isWorkflowAttached, WorkflowFieldName, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);

                                            foreach (ItemInfo newItem in _listItemsInfo)
                                            {
                                                _allItemInfo.Add(newItem);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            if (itemCount != 0)
                            {
                                _filteredItemInfo = _allItemInfo.OrderByDescending(si => si.Modified).Take(itemCount).ToList();
                            }
                            else
                            {
                                _filteredItemInfo = _allItemInfo.OrderByDescending(si => si.Modified).ToList();
                            }

                            foreach (ItemInfo item in _filteredItemInfo)
                            {
                                ItemInfo newItem = GetOtherItemDetails(item, siteURL);
                                _sortedItemInfo.Add(newItem);
                            }

                            jsonDataValue = new JavaScriptSerializer().Serialize(_sortedItemInfo);

                            object jsonDataObject = (object)jsonDataValue;
                            if (jsonDataValue != null) { CacheHelper.SaveTocache(cacheName, jsonDataObject); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                recentModifiedError = errMSG;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetRecentlyModifiedDocuments"), errMessage);
            }

            if (string.IsNullOrEmpty(jsonDataValue)) { jsonData = "{\"Results\":\"" + jsonDataValue + "\"}"; } else { jsonData = "{\"Results\":" + jsonDataValue + "}"; }
            string errorMessage = "{\"Message\":\"" + recentModifiedError + "\"}";
            string Result = string.Format("[{0},{1}]", jsonData, errorMessage);
            return Result;

        }

        /// <summary>
        /// Method to query the latest modified documents from list.
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="propListName"></param>
        /// <param name="rowLimit"></param>
        /// <param name="errMSG"></param>
        /// <returns></returns>
        public static SPListItemCollection GetRecentDocumentsFromCAMLQuery(string propSiteUrl, string propListName, int rowLimit, string errMSG)
        {
            string recentPracticeDocuments = string.Empty;
            SPListItemCollection items;
            try
            {
                using (SPSite site = new SPSite(propSiteUrl))
                {
                    using (SPWeb oWeb = site.OpenWeb())
                    {
                        SPList oList = oWeb.Lists.TryGetList(propListName);

                        SPQuery oQuery = new SPQuery();
                        if (Convert.ToInt32(rowLimit) != 0)
                        {
                            oQuery.ViewXml = "<View Scope=\"Recursive\"> " + "<Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + rowLimit + "</RowLimit></View>";
                        }
                        else
                        {
                            oQuery.ViewXml = "<View Scope=\"Recursive\"> " + "<Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                        }
                        items = oList.GetItems(oQuery);
                        SPGroupCollection groups = oWeb.SiteGroups;
                    }
                }
                return items;
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetRecentBestDocumentsFromCAMLQuery"), errMessage);
            }

            return null;
        }

        #endregion

        #region Frequently Used Documents

        /// <summary>
        /// Get Most Frequently used documents from site.
        /// </summary>
        /// <param name="siteURL">URL of the site</param>
        /// <param name="itemCount">Number of items to be fetched</param>        
        /// <returns></returns>
        [WebMethod]
        public static string GetFrequentlyUsedDocuments(string siteURL, int itemCount, string cacheName, string errMSG)
        {
            List<ItemInfo> _allItemInfo = new List<ItemInfo>();
            string jsonData = null, jsonDataValue = null, frequentlyUsedError = string.Empty;
            try
            {
                if (CacheHelper.IsIncache(cacheName))
                {
                    Object Cachereturn = (object)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { jsonDataValue = Cachereturn.ToString(); }
                }
                else
                {
                    DataTable resultDataTable;
                    using (SPSite site = new SPSite(siteURL))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {

                            string actualURL = string.Format(@"path:{0} AND IsDocument:true AND -fileextension:aspx", siteURL);

                            resultDataTable = GetFrequentlyUsedSearchResulsts(siteURL, itemCount, actualURL);
                            if (resultDataTable.Rows.Count <= 0)
                            {
                                if (actualURL.Contains("https://"))
                                {
                                    actualURL = actualURL.Replace("https://", "http://");
                                }
                                resultDataTable = GetFrequentlyUsedSearchResulsts(siteURL, itemCount, actualURL);
                            }


                            foreach (DataRow resultRow in resultDataTable.Rows)
                            {
                                ItemInfo newItem = new ItemInfo();
                                newItem.itemURL = resultRow[Constants.PATH].ToString();
                                {
                                    newItem.Title = resultRow[Constants.TITLE].ToString();
                                    newItem.Author = resultRow[Constants.AUTHOR].ToString();
                                    newItem.Modified = (DateTime)resultRow[Constants.LAST_MODIFIED_TIME];
                                    string formattedDate = string.Format("{0:dd MMM yyyy}", newItem.Modified);
                                    newItem.modifiedDate = formattedDate;
                                    if (resultRow[Constants.FILE_EXTENSION] != null)
                                    {
                                        newItem.fileExtension = resultRow[Constants.FILE_EXTENSION].ToString();
                                    }

                                    newItem.iconURL = CommonHelper.LoadRecentIcon(siteURL, newItem.itemURL);
                                    newItem.itemAbsoluteURL = newItem.itemURL;
                                    string webURL = siteURL;
                                    webURL = (!webURL.EndsWith("/")) ? webURL + "/" : webURL;
                                    string docEditURL = webURL + "_layouts/15/WopiFrame.aspx?sourcedoc=" + newItem.itemURL + "&action=default";
                                    if (newItem.fileExtension != null)
                                    {
                                        bool officeDoc = Constants.OFFICE_EXT.Any(newItem.fileExtension.Contains);
                                        newItem.itemURL = officeDoc ? docEditURL : newItem.itemURL;
                                    }
                                    _allItemInfo.Add(newItem);
                                }
                            }

                            jsonDataValue = new JavaScriptSerializer().Serialize(_allItemInfo);
                            object jsonDataObject = (object)jsonDataValue;
                            if (jsonDataValue != null) { CacheHelper.SaveTocache(cacheName, jsonDataObject); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                frequentlyUsedError = errMSG;
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetFrequentlyUsedDocuments"), errMessage);
            }

            if (string.IsNullOrEmpty(jsonDataValue)) { jsonData = "{\"Results\":\"" + jsonDataValue + "\"}"; } else { jsonData = "{\"Results\":" + jsonDataValue + "}"; }
            string errorMessage = "{\"Message\":\"" + frequentlyUsedError + "\"}";
            string Result = string.Format("[{0},{1}]", jsonData, errorMessage);
            return Result;
        }


        public static DataTable GetFrequentlyUsedSearchResulsts(string siteURL, int itemCount, string actualURL)
        {
            DataTable resultDataTable = null;
            try
            {

                KeywordQuery keywordQuery = new KeywordQuery();
                if (itemCount != 0)
                {
                    keywordQuery.RowLimit = itemCount;
                }
                keywordQuery.SortList.Add(Constants.SortViewsLifeTime, SortDirection.Descending);
                keywordQuery.QueryText = actualURL;
                keywordQuery.TrimDuplicates = true;
                SearchExecutor searchExecutor = new SearchExecutor();
                ResultTableCollection results = searchExecutor.ExecuteQuery(keywordQuery);
                var resultTables = results.Filter(Constants.ResultTableType, KnownTableTypes.RelevantResults);
                var resultTable = resultTables.FirstOrDefault();
                resultDataTable = resultTable.Table;
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodHelper", "GetFrequentlyUsedSearchResulsts"), errMessage);
            }
            return resultDataTable;
        }

        #endregion

    }
}
