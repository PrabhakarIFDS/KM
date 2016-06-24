using IFDS.KMPortal.Helper;
using IFDS.KMPortal.Models;
using Microsoft.Office.Server.Social;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace IFDS.KMPortal.Layouts.IFDS.KMPortal
{
    public static class util
    {
        public static bool IsIneumerableEmptyorNull<T>(this IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }

    public class ListDetails
    {
        public string WorkflowFieldName { get; set; }
        public bool isWorkflowAttached { get; set; }
        public bool IsModerationEnabled { get; set; }
    }

    public partial class WebMethodPage : LayoutsPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region Discussion Forum Section

        /// <summary>
        /// Retrieve Most Popular discussion by querying keyword search API
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

            List<DiscussionItemInfo> _discItemInfo = new List<DiscussionItemInfo>();
            try
            {
                if (CacheHelper.IsIncache(cacheName))
                {
                    List<DiscussionItemInfo> Cachereturn = (List<DiscussionItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _discItemInfo = Cachereturn; }
                }
                else
                {
                    bool isvalid = CommonHelper.IsListExsits(propListName, propItemCount, propSiteUrl, propErrorMessage);
                    if (isvalid)
                    {
                        using (ClientContext clientContext = new ClientContext(propSiteUrl))
                        {
                            //clientContext.Credentials = new NetworkCredential(Constants.USER_NAME, Constants.PWD);
                            Web oWeb = clientContext.Web;
                            clientContext.Load(oWeb);
                            clientContext.ExecuteQuery();
                            KeywordQuery keywordQuery = new KeywordQuery(clientContext);
                            Uri uri = new Uri(oWeb.Url.ToString());
                            string urlPath = System.Web.HttpUtility.UrlPathEncode(uri.GetLeftPart(UriPartial.Authority) + CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[0]);
                            string actualURL = string.Format(@"path:{0}", urlPath);
                            StringCollection selectProperties = keywordQuery.SelectProperties;
                            selectProperties.Add(Constants.CREATED);
                            selectProperties.Add(Constants.FILE_EXTENSION);
                            selectProperties.Add(Constants.CONTENT_TYPE_ID);
                            selectProperties.Add(Constants.TITLE);
                            selectProperties.Add(Constants.AUTHOR);
                            selectProperties.Add(Constants.PATH);
                            selectProperties.Add(Constants.REPLY_COUNT);
                            keywordQuery.QueryText = actualURL;
                            SearchExecutor searchExecutor = new SearchExecutor(clientContext);
                            ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
                            clientContext.ExecuteQuery();
                            _discItemInfo = GetAllPopularDiscussions(results, propItemCount, propErrorMessage);
                            if (_discItemInfo != null) { CacheHelper.SaveTocache(cacheName, _discItemInfo); }
                        }
                    }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_discItemInfo);

            }
            catch (Exception ex)
            {
                discErrMsg = propErrorMessage;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetMostPopularDiscussions"), ex.StackTrace);
            }
            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }
            viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            post = "{\"Post\":\"" + postValue + "\"}";
            string errorMessage = "{\"Message\":\"" + discErrMsg + "\"}";
            Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;
        }

        /// <summary>
        /// This method updated the DiscussionItemInfo properties by looping through the ListItemCollection.
        /// </summary>
        /// <param name="clientcontext"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<DiscussionItemInfo> GetAllPopularDiscussions(ClientResult<ResultTableCollection> items, string propDisplayCount, string propErrMsg)
        {
            List<DiscussionItemInfo> _alldiscItems = new List<DiscussionItemInfo>();

            try
            {
                if (items != null)
                {
                    int count = 0;

                    foreach (var resultRow in items.Value[0].ResultRows)
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
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetAllPopularDiscussions"), ex.StackTrace);
            }
            return _alldiscItems;
        }

        /// <summary>
        /// Retrieve Most Recent Discussion by querying discssion's list in the current site
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
            string WorkflowFieldName = string.Empty;
            bool isWorkflowAttached = false;
            bool enableModeration = false;
            bool isUserSiteAdmin = false;
            try
            {
                if (CacheHelper.IsIncache(cacheName))
                {
                    List<DiscussionItemInfo> Cachereturn = (List<DiscussionItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _discRecentItemInfo = Cachereturn; }
                }
                else
                {
                    bool isvalid = CommonHelper.IsListExsits(propListName, propItemCount, propSiteUrl, propErrorMessage);
                    if (isvalid)
                    {
                        using (ClientContext clientContext = new ClientContext(propSiteUrl))
                        {
                            //clientContext.Credentials = new NetworkCredential(Constants.USER_NAME, Constants.PWD);
                            List oList = clientContext.Web.Lists.GetByTitle(propListName);
                            CamlQuery camlQuery = new CamlQuery();
                            if (Convert.ToInt32(propItemCount) != 0)
                            {
                                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                            }
                            else
                            {
                                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                            }
                            ListItemCollection items = oList.GetItems(camlQuery);
                            clientContext.Load(items);
                            clientContext.Load(items, collection => collection.Include(item => item.RoleAssignments.Include(r => r.Member.LoginName, r => r.Member.Title)));
                            GroupCollection groups = clientContext.Web.SiteGroups;
                            clientContext.Load(groups, group => group.Include(g => g.Title, g => g.LoginName));
                            clientContext.ExecuteQuery();

                            string loggedInUsername = CommonHelper.DecodeUserName(userName);

                            //Is User Site Admin
                            isUserSiteAdmin = CommonHelper.IsUserSiteAdmin(clientContext, loggedInUsername);

                            //Get User Role assignment
                            List<string> userGroupArray = CommonHelper.GetUserGroups(clientContext, groups, loggedInUsername);

                            Dictionary<string, string> listDetails = CommonHelper.GetListWorkflowDetails(clientContext, propListName);
                            WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                            isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                            enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);
                            _discRecentItemInfo = GetAllRecentDiscussions(items, propItemCount, propErrorMessage, WorkflowFieldName, isWorkflowAttached, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);
                            if (_discRecentItemInfo != null) { CacheHelper.SaveTocache(cacheName, _discRecentItemInfo); }
                        }
                    }
                }

                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_discRecentItemInfo);

            }
            catch (Exception ex)
            {
                discErrMsg = propErrorMessage;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetMostRecentDiscussions"), ex.StackTrace);
            }

            if (string.IsNullOrEmpty(discitemsValue)) { discitems = "{\"Results\":\"" + discitemsValue + "\"}"; } else { discitems = "{\"Results\":" + discitemsValue + "}"; }
            viewAll = "{\"View\":\"" + viewAllValue + "\"}";
            post = "{\"Post\":\"" + postValue + "\"}";
            string errorMessage = "{\"Message\":\"" + discErrMsg + "\"}";
            Result = string.Format("[{0},{1},{2},{3}]", discitems, viewAll, post, errorMessage);
            return Result;
        }

        /// <summary>
        /// This method updates the discussioniteminfo properties by looping through the ListItemcollection.
        /// </summary>
        /// <param name="clientcontext"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<DiscussionItemInfo> GetAllRecentDiscussions(ListItemCollection items, string propDisplayCount, string propErrMsg, string WorkflowFieldName, bool isWorkflowAttached, bool enableModeration, string loggedInUsername, List<string> userGroups, bool IsUserSiteAdmin)
        {
            List<DiscussionItemInfo> _allRecentdiscItems = new List<DiscussionItemInfo>();
            bool addItem = true;
            try
            {
                if (items != null)
                {
                    foreach (ListItem oItem in items)
                    {
                        if (!string.IsNullOrEmpty(WorkflowFieldName))
                        {
                            addItem = CommonHelper.IsAddItem(isWorkflowAttached, enableModeration, oItem, WorkflowFieldName, loggedInUsername, userGroups, IsUserSiteAdmin);
                        }

                        if (addItem)
                        {
                            DiscussionItemInfo discRecentItem = new DiscussionItemInfo();
                            DateTime dt = (DateTime)oItem[Constants.CREATED];
                            string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                            discRecentItem.Created = formattedDate;
                            discRecentItem.Path = Uri.EscapeUriString(Convert.ToString(oItem[Constants.FILE_REF]));
                            discRecentItem.Title = Convert.ToString(oItem[Constants.TITLE]);
                            discRecentItem.Author = Convert.ToString(((FieldUserValue)oItem[Constants.AUTHOR]).LookupValue);
                            discRecentItem.ReplyCount = Convert.ToString(oItem[Constants.ITEM_CHILD_COUNT]);
                            _allRecentdiscItems.Add(discRecentItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetAllRecentDiscussions"), ex.StackTrace);
            }
            return _allRecentdiscItems;
        }

        #endregion

        #region FAQs Section

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
            try
            {
                if (CacheHelper.IsIncache(cacheName))
                {
                    List<FAQItemInfo> Cachereturn = (List<FAQItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _faqGlobalPopularItemInfo = Cachereturn; }
                }
                else
                {
                    bool isvalid = CommonHelper.IsListExsits(propListName, propItemCount, propSiteUrl, propErrorMessage);
                    if (isvalid)
                    {
                        using (ClientContext clientContext = new ClientContext(propSiteUrl))
                        {
                            List oList = clientContext.Web.Lists.GetByTitle(propListName);
                            CamlQuery camlQuery = new CamlQuery();
                            if (Convert.ToInt32(propItemCount) != 0)
                            {
                                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                            }
                            else
                            {
                                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                            }
                            ListItemCollection items = oList.GetItems(camlQuery);
                            clientContext.Load(items);
                            clientContext.Load(items, collection => collection.Include(i => i.ParentList.DefaultDisplayFormUrl, item => item.RoleAssignments.Include(r => r.Member.LoginName, r => r.Member.Title)));
                            //clientContext.Load(items, icol => icol.Include(i => i.ParentList.DefaultDisplayFormUrl));
                            GroupCollection groups = clientContext.Web.SiteGroups;
                            clientContext.Load(groups, group => group.Include(g => g.Title, g => g.LoginName));
                            clientContext.ExecuteQuery();

                            string loggedInUsername = CommonHelper.DecodeUserName(userName);

                            //Is User Site Admin
                            isUserSiteAdmin = CommonHelper.IsUserSiteAdmin(clientContext, loggedInUsername);

                            //Get User Role assignment
                            List<string> userGroupArray = CommonHelper.GetUserGroups(clientContext, groups, loggedInUsername);

                            Dictionary<string, string> listDetails = CommonHelper.GetListWorkflowDetails(clientContext, propListName);
                            string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                            bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                            bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                            _faqGlobalPopularItemInfo = GetGlobalPopularFAQ(items, propItemCount, propErrorMessage, WorkflowFieldName, isWorkflowAttached, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);
                            if (_faqGlobalPopularItemInfo != null) { CacheHelper.SaveTocache(cacheName, _faqGlobalPopularItemInfo); }
                        }
                    }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_faqGlobalPopularItemInfo);
            }
            catch (Exception ex)
            {
                faqErrMsg = propErrorMessage;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetMostPopularGlobalFAQData"), ex.StackTrace);
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
        public static List<FAQItemInfo> GetGlobalPopularFAQ(ListItemCollection items, string propDisplayCount, string propErrMsg, string WorkflowFieldName, bool isWorkflowAttached, bool enableModeration, string loggedInUserName, List<string> groups, bool isUserSiteAdmin)
        {
            List<FAQItemInfo> _allglobalPopularFaqItems = new List<FAQItemInfo>();
            bool addItem = true;
            try
            {
                if (items != null)
                {
                    foreach (ListItem oItem in items)
                    {
                        if (!string.IsNullOrEmpty(WorkflowFieldName))
                        {
                            addItem = CommonHelper.IsAddItem(isWorkflowAttached, enableModeration, oItem, WorkflowFieldName, loggedInUserName, groups, isUserSiteAdmin);
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
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetGlobalPopularFAQ"), ex.StackTrace);
            }
            return _allglobalPopularFaqItems;
        }

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
                if (CacheHelper.IsIncache(cacheName))
                {
                    List<FAQItemInfo> Cachereturn = (List<FAQItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _faqGlobalRecentItemInfo = Cachereturn; }
                }
                else
                {
                    bool isvalid = CommonHelper.IsListExsits(propListName, propItemCount, propSiteUrl, propErrorMessage);
                    if (isvalid)
                    {
                        using (ClientContext clientContext = new ClientContext(propSiteUrl))
                        {
                            List oList = clientContext.Web.Lists.GetByTitle(Constants.KMPORTAL_DEPT_SITES);
                            CamlQuery camlQuery = new CamlQuery();
                            if (Convert.ToInt32(propItemCount) != 0)
                            {
                                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                            }
                            else
                            {
                                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                            }
                            ListItemCollection items = oList.GetItems(camlQuery);
                            clientContext.Load(items);
                            clientContext.Load(items, icol => icol.Include(j => j.ParentList.DefaultDisplayFormUrl));
                            clientContext.ExecuteQuery();
                            _faqGlobalRecentItemInfo = GetGlobalRecentListItems(items, propErrorMessage, userName);
                            if (_faqGlobalRecentItemInfo != null) { CacheHelper.SaveTocache(cacheName, _faqGlobalRecentItemInfo); }
                        }
                    }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_faqGlobalRecentItemInfo);

            }
            catch (Exception ex)
            {
                faqErrMsg = propErrorMessage;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetMostRecentGlobalFAQData"), ex.StackTrace);
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
        public static List<FAQItemInfo> GetGlobalRecentListItems(ListItemCollection oItems, string propErrMsg, string userName)
        {
            List<FAQItemInfo> _allglobalrecentfaqItems = new List<FAQItemInfo>();
            bool isUserSiteAdmin = false;
            try
            {
                List<FAQRecentItemInfo> FinalList = new List<FAQRecentItemInfo>();
                foreach (ListItem oItem in oItems)
                {
                    string siteURL = Convert.ToString(oItem[Constants.SITE_URL]);
                    using (ClientContext oClientContext = new ClientContext(siteURL))
                    {
                        List oList = oClientContext.Web.Lists.GetByTitle(Constants.FAQLIST);
                        CamlQuery camlQuery = new CamlQuery();
                        camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + Constants.ROW_LIMIT + "</RowLimit></View>";
                        ListItemCollection items = oList.GetItems(camlQuery);
                        oClientContext.Load(items);
                        oClientContext.Load(items, collection => collection.Include(w => w.ParentList.DefaultDisplayFormUrl, w => w.ParentList.ParentWeb.Title, item => item.RoleAssignments.Include(r => r.Member.LoginName, r => r.Member.Title)));
                        GroupCollection groups = oClientContext.Web.SiteGroups;
                        oClientContext.Load(groups, group => group.Include(g => g.Title, g => g.LoginName));
                        oClientContext.ExecuteQuery();

                        string loggedInUsername = CommonHelper.DecodeUserName(userName);

                        //Is User Site Admin
                        isUserSiteAdmin = CommonHelper.IsUserSiteAdmin(oClientContext, loggedInUsername);

                        //Get User Role assignment
                        List<string> userGroupArray = CommonHelper.GetUserGroups(oClientContext, groups, loggedInUsername);


                        //List<ListItem> ResultList = new List<ListItem>();

                        Dictionary<string, string> listDetails = CommonHelper.GetListWorkflowDetails(oClientContext, Constants.FAQLIST);
                        string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                        bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                        bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);


                        foreach (ListItem item in items)
                        {

                            FAQRecentItemInfo ResultList = new FAQRecentItemInfo();

                            ResultList.DefaultDisplayFormUrl = item.ParentList.DefaultDisplayFormUrl;
                            ResultList.ID = Convert.ToString(item[Constants.ID]);
                            ResultList.ModifiedTime = (DateTime)item.FieldValues[Constants.MODIFIED];
                            ResultList.Answer = Convert.ToString(item.FieldValues[Constants.ANSWER]);
                            ResultList.Title = Convert.ToString(item.FieldValues[Constants.TITLE]);
                            ResultList.WorkflowFieldName = WorkflowFieldName;
                            ResultList.isWorkflowAttached = isWorkflowAttached;
                            ResultList.enableModeration = enableModeration;
                            ResultList.Item = item;
                            ResultList.DepartmentName = item.ParentList.ParentWeb.Title;
                            ResultList.loggedInUser = loggedInUsername;
                            ResultList.groupArray = userGroupArray;
                            ResultList.isUserSiteAdmin = isUserSiteAdmin;
                            FinalList.Add(ResultList);
                        }

                        //FinalList = FinalList.Concat(ResultList).ToList();
                    }
                }
                FinalList = FinalList.OrderByDescending(o => o.ModifiedTime).ToList();
                FinalList = FinalList.Take(5).ToList();


                foreach (FAQRecentItemInfo oItem in FinalList)
                {
                    bool addItem = true;
                    if (!string.IsNullOrEmpty(oItem.WorkflowFieldName))
                    {
                        addItem = CommonHelper.IsAddItem(oItem.isWorkflowAttached, oItem.enableModeration, oItem.Item, oItem.WorkflowFieldName, oItem.loggedInUser, oItem.groupArray, oItem.isUserSiteAdmin);
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
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetGlobalRecentListItems"), ex.StackTrace);
            }
            return _allglobalrecentfaqItems;
        }

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
                if (CacheHelper.IsIncache(cacheName))
                {
                    List<FAQItemInfo> Cachereturn = (List<FAQItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _faqdeptPopularItemInfo = Cachereturn; }
                }
                else
                {
                    bool isvalid = CommonHelper.IsListExsits(propListName, propItemCount, propSiteUrl, propErrorMessage);
                    if (isvalid)
                    {
                        using (ClientContext clientContext = new ClientContext(propSiteUrl))
                        {
                            Web oWeb = clientContext.Web;
                            clientContext.Load(oWeb);
                            clientContext.ExecuteQuery();
                            KeywordQuery keywordQuery = new KeywordQuery(clientContext);
                            Uri uri = new Uri(oWeb.Url.ToString());
                            string urlPath = System.Web.HttpUtility.UrlPathEncode(uri.GetLeftPart(UriPartial.Authority) + CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[0]);
                            string actualURL = string.Format(@"path:{0}", urlPath);
                            StringCollection selectProperties = keywordQuery.SelectProperties;
                            selectProperties.Add(Constants.LAST_MODIFIED_TIME);
                            selectProperties.Add(Constants.CONTENT_TYPE_ID);
                            selectProperties.Add(Constants.TITLE);
                            selectProperties.Add(Constants.PATH);
                            selectProperties.Add(Constants.ANSWER);
                            keywordQuery.QueryText = actualURL;
                            SearchExecutor searchExecutor = new SearchExecutor(clientContext);
                            ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
                            clientContext.ExecuteQuery();
                            _faqdeptPopularItemInfo = GetAllDeptPopularFAQ(results, propItemCount, propErrorMessage);
                            if (_faqdeptPopularItemInfo != null) { CacheHelper.SaveTocache(cacheName, _faqdeptPopularItemInfo); }
                        }
                    }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_faqdeptPopularItemInfo);

            }
            catch (Exception ex)
            {
                faqErrMsg = propErrorMessage;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetMostPopularDepartmentFAQData"), ex.StackTrace);
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
        /// This method updates the FAQItemInfo property  by looping through the ResultTableCollection
        /// </summary>
        /// <param name="items"></param>
        /// <param name="propDisplayCount"></param>
        /// <param name="propErrMsg"></param>
        /// <returns></returns>
        public static List<FAQItemInfo> GetAllDeptPopularFAQ(ClientResult<ResultTableCollection> items, string propDisplayCount, string propErrMsg)
        {
            List<FAQItemInfo> _alldeptfaqItems = new List<FAQItemInfo>();

            try
            {
                if (items != null)
                {
                    int count = 0;

                    foreach (var resultRow in items.Value[0].ResultRows)
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
                                faqDeptItem.Answer = CommonHelper.LimitString(Convert.ToString(resultRow[Constants.ANSWER]), Constants.CHAR_MAXLENGTH);
                                _alldeptfaqItems.Add(faqDeptItem);
                                count++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetAllDeptPopularFAQ"), ex.StackTrace);
            }
            return _alldeptfaqItems;
        }

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
            try
            {
                if (CacheHelper.IsIncache(cacheName))
                {
                    List<FAQItemInfo> Cachereturn = (List<FAQItemInfo>)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { _faqdeptRecentItemInfo = Cachereturn; }
                }
                else
                {
                    bool isvalid = CommonHelper.IsListExsits(propListName, propItemCount, propSiteUrl, propErrorMessage);
                    if (isvalid)
                    {
                        using (ClientContext clientContext = new ClientContext(propSiteUrl))
                        {
                            //clientContext.Credentials = new NetworkCredential(Constants.USER_NAME, Constants.PWD);
                            List oList = clientContext.Web.Lists.GetByTitle(propListName);
                            CamlQuery camlQuery = new CamlQuery();
                            if (Convert.ToInt32(propItemCount) != 0)
                            {
                                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propItemCount + "</RowLimit></View>";
                            }
                            else
                            {
                                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit></View>";
                            }
                            ListItemCollection items = oList.GetItems(camlQuery);
                            clientContext.Load(items);
                            clientContext.Load(items, collection => collection.Include(j => j.ParentList.DefaultDisplayFormUrl, item => item.RoleAssignments.Include(r => r.Member.LoginName, r => r.Member.Title)));
                            GroupCollection groups = clientContext.Web.SiteGroups;
                            clientContext.Load(groups, group => group.Include(g => g.Title, g => g.LoginName));
                            clientContext.ExecuteQuery();

                            string loggedInUsername = CommonHelper.DecodeUserName(userName);

                            //Is User Site Admin
                            isUserSiteAdmin = CommonHelper.IsUserSiteAdmin(clientContext, loggedInUsername);

                            //Get User Role assignment
                            List<string> userGroupArray = CommonHelper.GetUserGroups(clientContext, groups, loggedInUsername);

                            Dictionary<string, string> listDetails = CommonHelper.GetListWorkflowDetails(clientContext, propListName);
                            string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                            bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                            bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                            _faqdeptRecentItemInfo = GetAlldeptRecentFAQ(items, propItemCount, propErrorMessage, WorkflowFieldName, isWorkflowAttached, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);
                            if (_faqdeptRecentItemInfo != null) { CacheHelper.SaveTocache(cacheName, _faqdeptRecentItemInfo); }
                        }
                    }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, propSiteUrl)[1]);
                discitemsValue = new JavaScriptSerializer().Serialize(_faqdeptRecentItemInfo);
            }
            catch (Exception ex)
            {
                faqErrMsg = propErrorMessage;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetMostRecentDepartmentFAQData"), ex.StackTrace);
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
        public static List<FAQItemInfo> GetAlldeptRecentFAQ(ListItemCollection items, string propDisplayCount, string propErrMsg, string WorkflowFieldName, bool isWorkflowAttached, bool enableModeration, string loggedInUser, List<string> groups, bool IsUserSiteAdmin)
        {
            List<FAQItemInfo> _alldeptRecentFaqItems = new List<FAQItemInfo>();
            bool addItem = true;
            try
            {
                if (items != null)
                {
                    foreach (ListItem oItem in items)
                    {

                        if (!string.IsNullOrEmpty(WorkflowFieldName))
                        {
                            addItem = CommonHelper.IsAddItem(isWorkflowAttached, enableModeration, oItem, WorkflowFieldName, loggedInUser, groups, IsUserSiteAdmin);
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
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetAlldeptRecentFAQ"), ex.StackTrace);
            }
            return _alldeptRecentFaqItems;
        }

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
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetMyFavouritesData"), ex.StackTrace);
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
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethod", "GetAllMyFavouriteListItems"), ex.StackTrace);
            }
            return _allmyfavItems;
        }

        #endregion

        #region TopContributors and Learner's Section

        /// <summary>
        /// Get Most Recent Data from News and Announcement visual webpart
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetTopContributors(string siteurl, string topcontributorslistname, int rowlimit)
        {

            List<TopContributors> tp = new List<TopContributors>();

            try
            {
                using (ClientContext clientContext = new ClientContext(siteurl))
                {
                    Web site = clientContext.Web;

                    List targetList = site.Lists.GetByTitle(topcontributorslistname);
                    //ViewCollection collView = targetList.Views;
                    //View tview = collView.GetByTitle("Members View");
                    //tview.Update();
                    CamlQuery bamlQuery = new CamlQuery();
                    bamlQuery.ViewXml = "<View><OrderBy><FieldRef Name='ReputationScore' Ascending='False'/></OrderBy><RowLimit>" + rowlimit + "</RowLimit></View>";
                    ListItemCollection collListItem = targetList.GetItems(bamlQuery);


                    clientContext.Load(collListItem, items => items.Include(
                                             item => item["Member"],
                                             item => item["NumberOfDiscussions"],
                                             item => item["NumberOfReplies"],
                                             item => item["GiftedBadgeText"],
                                             item => item["ReputationScore"]
                                             ));
                    clientContext.ExecuteQuery();

                    foreach (ListItem li in collListItem)
                    {
                        TopContributors listitem = new TopContributors();
                        listitem.Member = ((Microsoft.SharePoint.Client.FieldUserValue)(li["Member"])).LookupValue;
                        listitem.NoOfDiscussions = Convert.ToInt32(li["NumberOfDiscussions"]);
                        listitem.NoOfReplies = Convert.ToInt32(li["NumberOfReplies"]);
                        listitem.GiftedBadgeText = Convert.ToString(li["GiftedBadgeText"]);
                        listitem.ReputationScore = Convert.ToInt32(li["ReputationScore"]);

                        ClientResult<Microsoft.SharePoint.Client.Utilities.PrincipalInfo> persons = Microsoft.SharePoint.Client.Utilities.Utility.ResolvePrincipal(clientContext, clientContext.Web, listitem.Member, Microsoft.SharePoint.Client.Utilities.PrincipalType.User, Microsoft.SharePoint.Client.Utilities.PrincipalSource.All, null, true);
                        clientContext.ExecuteQuery();
                        Microsoft.SharePoint.Client.Utilities.PrincipalInfo person = persons.Value;


                        Microsoft.SharePoint.Client.UserProfiles.PeopleManager peopleManager = new Microsoft.SharePoint.Client.UserProfiles.PeopleManager(clientContext);

                        // Retrieve specific properties by using the GetUserProfilePropertiesFor method. 
                        // The returned collection contains only property values.
                        string[] profilePropertyNames = new string[] { "WorkEmail", "WorkPhone", "Title", "PersonalSpace" };
                        string[] domainname = person.LoginName.Split('|');
                        string userstring = domainname.Length == 2 ? domainname[1] : domainname[0];

                        Microsoft.SharePoint.Client.UserProfiles.UserProfilePropertiesForUser profilePropertiesForUser = new Microsoft.SharePoint.Client.UserProfiles.UserProfilePropertiesForUser(clientContext, userstring, profilePropertyNames);
                        profilePropertiesForUser.AccountName = userstring;
                        IEnumerable<string> profilePropertyValues = peopleManager.GetUserProfilePropertiesFor(profilePropertiesForUser);
                        //ArrayList returnvalues = ArrayList.Repeat(null, 2);

                        // Load the request and run it on the server.
                        clientContext.Load(profilePropertiesForUser);
                        clientContext.ExecuteQuery();

                        if (util.IsIneumerableEmptyorNull<string>(profilePropertyValues))
                        {
                            ArrayList profilevalues = new ArrayList(5);
                            foreach (var s in profilePropertyValues) profilevalues.Add(s.ToString());
                            if (profilevalues.Count > 0)
                            {
                                listitem.emailaddress = profilevalues[0].ToString();
                                listitem.phonenumber = profilevalues[1].ToString();
                                listitem.personalurl = profilevalues[3].ToString();
                            }
                        }
                        tp.Add(listitem);
                    }
                }
                return new JavaScriptSerializer().Serialize(tp);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, "Top Contributors and Learner webpart", "Error querying external list" + topcontributorslistname);
                return ex.Message.ToString();
            }

        }


        /// <summary>
        /// Get Most Recent Data from News and Announcement visual webpart
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetTopLearners(string siteurl, string toplearnerslistname, string departmentname, int rowlimit)
        {

            List<TopLearnersTable> tp = new List<TopLearnersTable>();

            try
            {
                using (ClientContext clientContext = new ClientContext(siteurl))
                {
                    Web site = clientContext.Web;

                    List targetList = site.Lists.GetByTitle(toplearnerslistname);
                    //ViewCollection collView = targetList.Views;
                    //View tview = collView.GetByTitle("New external content Read List");
                    //tview.Update();
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='web' /><Value Type='Text'>" + departmentname + "</Value></Eq></Where><OrderBy><FieldRef Name='ReputationScore' Ascending='FALSE'/></OrderBy><ViewFields><FieldRef Name='ReputationScore'/><FieldRef Name='Member'/><FieldRef Name='MemberStatus'/><FieldRef Name='web'/><FieldRef Name='Displayname'/><FieldRef Name='Column1'/><FieldRef Name='Column2'/></ViewFields></Query><RowLimit>" + rowlimit + "</RowLimit></View>";
                    ListItemCollection collListItem = targetList.GetItems(camlQuery);


                    clientContext.Load(collListItem);
                    clientContext.ExecuteQuery();

                    if (collListItem.Count > 0)
                    {

                        foreach (ListItem listitem in collListItem)
                        {
                            TopLearnersTable topltablerow = new TopLearnersTable();
                            topltablerow.Member = Convert.ToString(listitem["Member"]);
                            topltablerow.MemberStatus = Convert.ToString(listitem["MemberStatus"]);
                            topltablerow.displayname = Convert.ToString(listitem["Displayname"]);
                            topltablerow.Department = Convert.ToString(listitem["web"]);
                            topltablerow.ReputationScore = Convert.ToInt32(listitem["ReputationScore"]);
                            topltablerow.column1 = Convert.ToInt32(listitem["Column1"]);
                            topltablerow.column2 = Convert.ToInt32(listitem["Column2"]);

                            ClientResult<Microsoft.SharePoint.Client.Utilities.PrincipalInfo> persons = Microsoft.SharePoint.Client.Utilities.Utility.ResolvePrincipal(clientContext, clientContext.Web, topltablerow.Member, Microsoft.SharePoint.Client.Utilities.PrincipalType.User, Microsoft.SharePoint.Client.Utilities.PrincipalSource.All, null, true);
                            clientContext.ExecuteQuery();
                            Microsoft.SharePoint.Client.Utilities.PrincipalInfo person = persons.Value;

                            Microsoft.SharePoint.Client.UserProfiles.PeopleManager peopleManager = new Microsoft.SharePoint.Client.UserProfiles.PeopleManager(clientContext);

                            // Retrieve specific properties by using the GetUserProfilePropertiesFor method. 
                            // The returned collection contains only property values.
                            string[] profilePropertyNames = new string[] { "WorkEmail", "WorkPhone", "Title", "PersonalSite" };
                            string[] domainname = person.LoginName.Split('|');
                            string userstring = domainname.Length == 2 ? domainname[1] : domainname[0];
                            Microsoft.SharePoint.Client.UserProfiles.UserProfilePropertiesForUser profilePropertiesForUser = new Microsoft.SharePoint.Client.UserProfiles.UserProfilePropertiesForUser(
                                clientContext, userstring, profilePropertyNames);
                            IEnumerable<string> profilePropertyValues = peopleManager.GetUserProfilePropertiesFor(profilePropertiesForUser);
                            //ArrayList returnvalues = ArrayList.Repeat( null, 2);

                            // Load the request and run it on the server.
                            clientContext.Load(profilePropertiesForUser);
                            clientContext.ExecuteQuery();

                            if (util.IsIneumerableEmptyorNull<string>(profilePropertyValues))
                            {
                                ArrayList profilevalues = new ArrayList(5);
                                foreach (var s in profilePropertyValues) profilevalues.Add(s.ToString());
                                if (profilevalues.Count > 0)
                                {
                                    topltablerow.emailaddress = profilevalues[0].ToString();
                                    topltablerow.phonenumber = profilevalues[1].ToString();
                                    topltablerow.personalurl = profilevalues[3].ToString();
                                }
                            }
                            tp.Add(topltablerow);
                        }


                    }

                }
                return new JavaScriptSerializer().Serialize(tp);

            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, "Top Contributors and Learner webpart", "Error querying external list" + toplearnerslistname);
                return ex.Message.ToString();
            }



            //return "[{'Error' : 'true'}]";

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
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethodPage", "GetSQLConnection"), ex.StackTrace);
            }
            return con;
            
        }

        #endregion


        #region Recent News Documents

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
                if (CacheHelper.IsIncache(cacheName))
                {
                    Object Cachereturn = (object)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { jsonDataValue = Cachereturn.ToString(); }
                }
                else
                {
                    using (ClientContext clientContext = new ClientContext(siteURL))
                    {
                        //clientContext.Credentials = new NetworkCredential(Constants.USER_NAME, Constants.PWD);
                        List oList = clientContext.Web.Lists.GetByTitle(listName);
                        CamlQuery camlQuery = new CamlQuery();
                        camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + itemCount + "</RowLimit></View>";
                        ListItemCollection items = oList.GetItems(camlQuery);
                        clientContext.Load(items);
                        clientContext.ExecuteQuery();

                        Dictionary<string, string> listDetails = CommonHelper.GetListWorkflowDetails(clientContext, listName);
                        string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                        bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                        bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                        _newsItemInfo = GetNewsDocuments(items, clientContext, clientContext.Url, isWorkflowAttached, WorkflowFieldName, enableModeration, defaultImageUrl);
                    }
                    jsonDataValue = new JavaScriptSerializer().Serialize(_newsItemInfo);
                    object jsonDataObject = (object)jsonDataValue;
                    if (jsonDataValue != null) { CacheHelper.SaveTocache(cacheName, jsonDataObject); }
                }

                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(listName, siteURL)[0]);
                postValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(listName, siteURL)[1]);
            }
            catch (Exception ex)
            {
                mostRecentNews = errMSG;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "CommonHelper", "GetRecentNewsFromCAMLQuery"), ex.StackTrace);
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
        public static List<NewsItemInfo> GetNewsDocuments(ListItemCollection items, ClientContext clientContext, string siteURL, bool isWorkflowAttached, string fieldName, bool enableModeration, string defaultImageUrl)
        {
            List<NewsItemInfo> _ItemInfo = new List<NewsItemInfo>();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                //  bool addItem = CommonHelper.IsAddItem(isWorkflowAttached, enableModeration, item, fieldName, "", null);
                bool addItem = true;
                if (addItem)
                {
                    NewsItemInfo newItem = new NewsItemInfo();
                    if (item[Constants.TITLE] != null)
                    {
                        newItem.Title = item[Constants.TITLE].ToString();
                    }
                    newItem.FileDirRef = item[Constants.FILE__DIR_REF].ToString();
                    newItem.ID = item[Constants.ID].ToString();
                    newItem.Author = ((FieldUserValue)item[Constants.AUTHOR]).LookupValue.ToString();
                    newItem.Modified = (DateTime)item[Constants.MODIFIED];

                    if (item[Constants.IMAGE] != null)
                    {
                        FieldUrlValue imgfield = (FieldUrlValue)item[Constants.IMAGE];

                        if (imgfield != null)
                            newItem.imageURL = imgfield.Url;
                    }
                    else newItem.imageURL = defaultImageUrl;

                    newItem.itemURL = newItem.FileDirRef + "/DispForm.aspx?ID=" + newItem.ID;
                    newItem.modifiedDate = string.Format("{0:dd MMM yyyy}", newItem.Modified);

                    _ItemInfo.Add(newItem);
                }
            }

            return _ItemInfo;
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
                if (CacheHelper.IsIncache(cacheName))
                {
                    Object Cachereturn = (object)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { jsonDataValue = Cachereturn.ToString(); }
                }
                else
                {
                    using (ClientContext clientContext = new ClientContext(siteURL))
                    {
                        Web oWeb = clientContext.Web;
                        clientContext.Credentials = CredentialCache.DefaultNetworkCredentials;
                        List oList = clientContext.Web.Lists.GetByTitle(propListName);
                        CamlQuery camlQuery = new CamlQuery();
                        camlQuery.ViewXml = "<View Scope=\"Recursive\"><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propDisplayCount + "</RowLimit></View>";

                        ListItemCollection items = oList.GetItems(camlQuery);
                        clientContext.Load(items);
                        clientContext.Load(items, collection => collection.Include(item => item.RoleAssignments.Include(r => r.Member.LoginName, r => r.Member.Title)));
                        GroupCollection groups = clientContext.Web.SiteGroups;
                        clientContext.Load(groups, group => group.Include(g => g.Title, g => g.LoginName));
                        clientContext.ExecuteQuery();

                        string loggedInUsername = CommonHelper.DecodeUserName(userName);

                        //Is User Site Admin
                        isUserSiteAdmin = CommonHelper.IsUserSiteAdmin(clientContext, loggedInUsername);

                        //Get User Role assignment
                        if (!isUserSiteAdmin)
                        {
                            userGroupArray = CommonHelper.GetUserGroups(clientContext, groups, loggedInUsername);
                        }

                        Dictionary<string, string> listDetails = CommonHelper.GetListWorkflowDetails(clientContext, propListName);
                        string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                        bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                        bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                        List<ItemInfo> _bestPracticeItemInfo = GetDocuments(items, clientContext, clientContext.Url, isWorkflowAttached, WorkflowFieldName, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);


                        foreach (ItemInfo item in _bestPracticeItemInfo)
                        {
                            ItemInfo newItem = GetOtherItemDetails(item, clientContext, siteURL);
                            _sortedBestPracticeItemInfo.Add(newItem);

                        }
                    }

                    jsonDataValue = new JavaScriptSerializer().Serialize(_sortedBestPracticeItemInfo);
                    object jsonDataObject = (object)jsonDataValue;
                    if (jsonDataValue != null) { CacheHelper.SaveTocache(cacheName, jsonDataObject); }
                }
                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, siteURL)[0]);
                uploadValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, siteURL)[2]);
            }
            catch (Exception ex)
            {
                recentPracticeDocuments = errMSG;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "CommonHelper", "GetRecentBestDocuments"), ex.StackTrace);
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
        public static List<ItemInfo> GetDocuments(ListItemCollection items, ClientContext clientContext, string siteURL, bool isWorkflowAttached, string fieldName, bool enableModeration, string loggedInUsername, List<string> userGroups, bool isUserSiteAdmin)
        {
            List<ItemInfo> _ItemInfo = new List<ItemInfo>();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                ItemInfo newItem = null;

                bool addItem = CommonHelper.IsAddItem(isWorkflowAttached, enableModeration, item, fieldName, loggedInUsername, userGroups, isUserSiteAdmin);
                if (addItem)
                {
                    newItem = GetDocumentItem(item, clientContext, siteURL);
                }

                if (newItem != null)
                {
                    _ItemInfo.Add(newItem);

                }
            }

            return _ItemInfo;
        }

        /// <summary>
        /// Get the document item properties.
        /// </summary>       
        /// <returns></returns>
        public static ItemInfo GetDocumentItem(ListItem item, ClientContext clientContext, string siteURL)
        {
            ItemInfo newItem = new ItemInfo();
            if (item[Constants.TITLE] != null)
            {
                newItem.Title = item[Constants.TITLE].ToString();
            }
            newItem.FileLeafRef = item[Constants.FILE__LEAF_REF].ToString();
            newItem.FileRef = item[Constants.FILE_REF].ToString();
            newItem.Author = ((FieldUserValue)item[Constants.AUTHOR]).LookupValue.ToString();
            newItem.Modified = (DateTime)item[Constants.MODIFIED];
            if (item[Constants.OFFICE_EXT_TYPE] != null)
            {
                newItem.fileExtension = item[Constants.OFFICE_EXT_TYPE].ToString();
            }

            return newItem;
        }


        /// <summary>
        /// Get the item properties of the list
        /// </summary>
        /// <returns></returns>
        public static ItemInfo GetOtherItemDetails(ItemInfo newItem, ClientContext clientContext, string siteURL)
        {
            newItem.iconURL = CommonHelper.LoadIconRecent(clientContext, newItem.FileLeafRef);
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
                if (CacheHelper.IsIncache(cacheName))
                {
                    Object Cachereturn = (object)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { jsonDataValue = Cachereturn.ToString(); }
                }
                else
                {
                    using (ClientContext clientContext = new ClientContext(siteURL))
                    {
                        Web oWeb = clientContext.Web;
                        //clientContext.Credentials = new NetworkCredential(Constants.USER_NAME, Constants.PWD);
                        List oList = clientContext.Web.Lists.GetByTitle(propListName);
                        CamlQuery camlQuery = new CamlQuery();
                        camlQuery.ViewXml = "<View Scope=\"Recursive\"><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propDisplayCount + "</RowLimit></View>";
                        ListItemCollection items = oList.GetItems(camlQuery);
                        clientContext.Load(items);
                        clientContext.Load(items, collection => collection.Include(item => item.RoleAssignments.Include(r => r.Member.LoginName, r => r.Member.Title)));
                        GroupCollection groups = clientContext.Web.SiteGroups;
                        clientContext.Load(groups, group => group.Include(g => g.Title, g => g.LoginName));
                        clientContext.ExecuteQuery();

                        string loggedInUsername = CommonHelper.DecodeUserName(userName);

                        //Is User Site Admin
                        isUserSiteAdmin = CommonHelper.IsUserSiteAdmin(clientContext, loggedInUsername);

                        //Get User Role assignment
                        if (!isUserSiteAdmin)
                        {
                            userGroupArray = CommonHelper.GetUserGroups(clientContext, groups, loggedInUsername);
                        }

                        Dictionary<string, string> listDetails = CommonHelper.GetListWorkflowDetails(clientContext, propListName);
                        string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                        bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                        bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                        List<ItemInfo> _onBoardingItemInfo = GetDocuments(items, clientContext, clientContext.Url, isWorkflowAttached, WorkflowFieldName, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);

                        foreach (ItemInfo item in _onBoardingItemInfo)
                        {
                            ItemInfo newItem = GetOtherItemDetails(item, clientContext, siteURL);
                            _sortedOnBoardingItemInfo.Add(newItem);
                        }
                    }

                    jsonDataValue = new JavaScriptSerializer().Serialize(_sortedOnBoardingItemInfo);
                    object jsonDataObject = (object)jsonDataValue;
                    if (jsonDataValue != null) { CacheHelper.SaveTocache(cacheName, jsonDataObject); }
                }

                viewAllValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, siteURL)[0]);
                uploadValue = Uri.EscapeUriString(CommonHelper.GetListUrlByTitle(propListName, siteURL)[2]);
            }
            catch (Exception ex)
            {
                recentOnboarding = errMSG;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "CommonHelper", "GetRecentOnboardingDocuments"), ex.StackTrace);
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
                if (CacheHelper.IsIncache(cacheName))
                {
                    Object Cachereturn = (object)CacheHelper.GetFromCache(cacheName);
                    if (Cachereturn != null) { jsonDataValue = Cachereturn.ToString(); }
                }
                else
                {
                    using (ClientContext clientContext = new ClientContext(siteURL))
                    {
                        Web oWeb = clientContext.Web;
                        ListCollection lists = clientContext.Web.Lists;
                        GroupCollection groups = clientContext.Web.SiteGroups;
                        clientContext.Load(lists);
                        clientContext.Load(groups, group => group.Include(g => g.Title, g => g.LoginName));
                        clientContext.ExecuteQuery();

                        string loggedInUsername = CommonHelper.DecodeUserName(userName);

                        //Is User Site Admin
                        isUserSiteAdmin = CommonHelper.IsUserSiteAdmin(clientContext, loggedInUsername);

                        //Get User Role assignment
                        if (!isUserSiteAdmin)
                        {
                            userGroupArray = CommonHelper.GetUserGroups(clientContext, groups, loggedInUsername);
                        }

                        ListItemCollection items = null;
                        string[] listArray = Constants.LibrariesToExclude.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        #region Loop all the lists
                        foreach (List list in lists)
                        {
                            string listName = list.Title;

                            if (!list.Hidden && list.BaseType == BaseType.DocumentLibrary)
                            {
                                if (!listArray.Contains(listName))
                                {
                                    //Query the list for recent documents.
                                    items = GetRecentDocumentsFromCAMLQuery(clientContext, list.Title, itemCount, "Error message");

                                    //Get Workflow details 
                                    Dictionary<string, string> listDetails = CommonHelper.GetListWorkflowDetails(clientContext, listName);
                                    string WorkflowFieldName = listDetails[Constants.WorkflowFieldName];
                                    bool isWorkflowAttached = Convert.ToBoolean(listDetails[Constants.IsWorkflowAttached]);
                                    bool enableModeration = Convert.ToBoolean(listDetails[Constants.IsModerationEnabled]);

                                    List<ItemInfo> _listItemsInfo = GetDocuments(items, clientContext, clientContext.Url, isWorkflowAttached, WorkflowFieldName, enableModeration, loggedInUsername, userGroupArray, isUserSiteAdmin);

                                    foreach (ItemInfo newItem in _listItemsInfo)
                                    {
                                        _allItemInfo.Add(newItem);
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
                            ItemInfo newItem = GetOtherItemDetails(item, clientContext, siteURL);
                            _sortedItemInfo.Add(newItem);
                        }

                        jsonDataValue = new JavaScriptSerializer().Serialize(_sortedItemInfo);

                        object jsonDataObject = (object)jsonDataValue;
                        if (jsonDataValue != null) { CacheHelper.SaveTocache(cacheName, jsonDataObject); }
                    }
                }
            }
            catch (Exception ex)
            {
                recentModifiedError = errMSG;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "WebMethods", "GetRecentlyModifiedDocuments"), ex.StackTrace);
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
        public static ListItemCollection GetRecentDocumentsFromCAMLQuery(ClientContext clientContext, string propListName, int rowLimit, string errMSG)
        {
            string recentPracticeDocuments = string.Empty;
            try
            {
                List oList = clientContext.Web.Lists.GetByTitle(propListName);
                CamlQuery camlQuery = new CamlQuery();
                if (rowLimit != 0)
                {
                    camlQuery.ViewXml = "<View Scope=\"Recursive\"> " + "<Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + rowLimit + "</RowLimit></View>";
                }
                else
                {
                    camlQuery.ViewXml = "<View Scope=\"Recursive\"> " + "<Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                }

                ListItemCollection items = oList.GetItems(camlQuery);
                clientContext.Load(items);
                clientContext.Load(items, collection => collection.Include(item => item.RoleAssignments.Include(r => r.Member.LoginName, r => r.Member.Title)));
                clientContext.ExecuteQuery();

                return items;

            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "WebMethods", "GetRecentBestDocuments"), ex.StackTrace);
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
                    ClientContext clientContext = new ClientContext(siteURL);
                    Web oWeb = clientContext.Web;
                    clientContext.Load(oWeb);
                    clientContext.ExecuteQuery();
                    KeywordQuery keywordQuery = new KeywordQuery(clientContext);
                    Uri uri = new Uri(oWeb.Url.ToString());
                    string urlPath = siteURL;

                    string actualURL = string.Format(@"path:{0} AND IsDocument:true AND -fileextension:aspx", urlPath);
                    if (itemCount != 0)
                    {
                        keywordQuery.RowLimit = itemCount;
                    }
                    keywordQuery.SortList.Add(Constants.SortViewsLifeTime, SortDirection.Descending);

                    //path:http://cad2ca1vspw03/sites/testing3'&rowlimit=5&sortlist='ViewsLifeTime:descending'

                    keywordQuery.QueryText = actualURL;
                    keywordQuery.TrimDuplicates = true;
                    SearchExecutor searchExecutor = new SearchExecutor(clientContext);
                    ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
                    clientContext.ExecuteQuery();


                    ClientResult<ResultTableCollection> items = results;
                    foreach (var resultRow in items.Value[0].ResultRows)
                    {
                        ItemInfo newItem = new ItemInfo();
                        newItem.itemURL = resultRow[Constants.PATH].ToString();
                        //if (!newItem.itemURL.Contains("/Pages/") && !newItem.itemURL.Contains("/SitePages/"))
                        {
                            newItem.Title = resultRow[Constants.TITLE].ToString();
                            newItem.Author = resultRow[Constants.AUTHOR].ToString();
                            newItem.Modified = (DateTime)resultRow[Constants.LAST_MODIFIED_TIME];
                            string formattedDate = string.Format("{0:dd MMM yyyy}", newItem.Modified);
                            newItem.modifiedDate = formattedDate;
                            if (resultRow["FileExtension"] != null)
                            {
                                newItem.fileExtension = resultRow["FileExtension"].ToString();
                            }

                            newItem.iconURL = CommonHelper.LoadIconRecent(clientContext, newItem.itemURL);
                            newItem.itemAbsoluteURL = newItem.itemURL;
                            string webURL = clientContext.Url;
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
            catch (Exception ex)
            {
                frequentlyUsedError = errMSG;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-WebMethods", "GetFrequentlyUsedDocuments"), ex.StackTrace);
            }

            if (string.IsNullOrEmpty(jsonDataValue)) { jsonData = "{\"Results\":\"" + jsonDataValue + "\"}"; } else { jsonData = "{\"Results\":" + jsonDataValue + "}"; }
            string errorMessage = "{\"Message\":\"" + frequentlyUsedError + "\"}";
            string Result = string.Format("[{0},{1}]", jsonData, errorMessage);
            return Result;
        }

        #endregion
    }
}
