#region Namespaces
using Microsoft.Office.Server.Social;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Workflow;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace IFDS.KMPortal.Helper
{
    public static class CommonHelper
    {

        #region CSOM Section

        #region Global Declaration Section

        /// <summary>
        /// Global variable declaration Section.
        /// </summary>
        static ClientContext clientContext;

        #endregion

        #region Common Section

        /// <summary>
        /// This method loads the clientcontext and check whether the list exsists in the current context or not.
        /// </summary>
        /// <returns></returns>
        public static bool GetClientContext(string propListName, string propItemCount, string propSiteUrl, string propErrMsg)
        {
            bool isExists = false;
            try
            {
                clientContext = new ClientContext(propSiteUrl);
                bool isvalid = GetListByTitle(propListName, clientContext) ? isExists = true : isExists = false;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetClientContext"), ex.StackTrace);
            }
            return isExists;
        }

        /// <summary>
        /// This method loads the clientcontext and check whether the list exsists in the current context or not.
        /// </summary>
        /// <returns></returns>
        public static bool IsListExsits(string propListName, string propItemCount, string propSiteUrl, string propErrMsg)
        {
            bool isExists = false;
            try
            {
                using (ClientContext clientContext = new ClientContext(propSiteUrl))
                {
                    bool isvalid = GetListByTitle(propListName, clientContext) ? isExists = true : isExists = false;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "IsListExsits"), ex.StackTrace);
            }
            return isExists;
        }

        /// <summary>
        /// This method validate if thee list exists in sharepoint site.
        /// returns true if exists elsereturns false.
        /// </summary>
        /// <param name="clientcontext"></param>
        /// <param name="listTitle"></param>
        /// <returns></returns>
        public static bool GetListByTitle(String listTitle, ClientContext clientContext)
        {
            bool isValidList = false;
            try
            {
                List existingList;
                Web web = clientContext.Web;
                ListCollection lists = web.Lists;
                IEnumerable<List> existingLists = clientContext.LoadQuery(
                        lists.Where(
                        list => list.Title == listTitle)
                        );
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                clientContext.ExecuteQuery();
                existingList = existingLists.FirstOrDefault();
                if (existingList != null)
                {
                    isValidList = true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetListByTitle"), ex.StackTrace);
            }
            return isValidList;
        }

        /// <summary>
        /// This method returns list sever relative url.
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public static string[] GetListUrlByTitle(string propListName, string propSiteUrl)
        {
            string[] listURL = new string[3];
            try
            {
                using (ClientContext clientContext = new ClientContext(propSiteUrl))
                {
                    //clientContext.Credentials = new NetworkCredential(Constants.USER_NAME, Constants.PWD);
                    Web oWeb = clientContext.Web;
                    List oList = clientContext.Web.Lists.GetByTitle(propListName);
                    clientContext.Load(oList.RootFolder);
                    clientContext.Load(oList);
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    clientContext.ExecuteQuery();
                    listURL[0] = oList.RootFolder.ServerRelativeUrl;
                    listURL[1] = oList.RootFolder.ServerRelativeUrl + "/" + Constants.NEWFORM;
                    string webURL = string.Empty;
                    webURL = (!oWeb.Context.Url.EndsWith("/")) ? oWeb.Context.Url + "/" : oWeb.Context.Url;
                    string listUrl = string.Empty;
                    if (oList.BaseType == BaseType.DocumentLibrary)
                        listUrl = webURL + oList.RootFolder.Name;
                    else
                        listUrl = webURL + "Lists/" + oList.RootFolder.Name;
                    listURL[2] = webURL + "_layouts/15/Upload.aspx?List={" + oList.Id + "}&RootFolder=&Source=" + listUrl + "";
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetListUrlByTitle"), ex.StackTrace);
            }

            return listURL;
        }

        /// <summary>
        /// Limit Text to 300 char
        /// </summary>
        /// <param name="s"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static string LimitString(string str, int maxLength)
        {
            string resultString = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    resultString = str.Length <= maxLength ? str : string.Format("{0}[...]", str.Remove(maxLength));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "LimitString"), ex.StackTrace);
            }
            return resultString;
        }

        /// <summary>
        /// Get filename without extension
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(string filename)
        {
            string fileNameWithoutExtension = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    string extension = System.IO.Path.GetExtension(filename);
                    fileNameWithoutExtension = filename.Substring(0, filename.Length - extension.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetFileNameWithoutExtension"), ex.StackTrace);
            }

            return fileNameWithoutExtension;
        }

        /// <summary>
        /// This method returns the name of the image file for the icon that is used to represent the specified file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="propSiteURL"></param>
        /// <returns></returns>
        public static string LoadIcon(string fileName, string propSiteURL)
        {
            string icon = string.Empty;
            try
            {
                using (SPSite site = new SPSite(propSiteURL))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        icon = SPUtility.MapToIcon(web, fileName, string.Empty, IconSize.Size16);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "LoadIcon"), ex.StackTrace);
            }
            return icon;
        }

        /// <summary>
        /// Get ListItem Properties of a document by fileref
        /// </summary>
        /// <param name="fileRef"></param>
        /// <returns></returns>
        public static string[] GetDocListItemProperties(string fileRef)
        {
            string[] DocItemProp = new string[3];
            try
            {
                using (SPSite site = new SPSite(fileRef))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFile file = web.GetFile(fileRef);
                        if (file.Exists)
                        {
                            DocItemProp[0] = file.Item[Constants.MODIFIED].ToString();
                            DocItemProp[1] = file.Item[Constants.AUTHOR].ToString().Split('#')[1];
                            DocItemProp[2] = file.Item.ParentList.ParentWeb.Title;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetDocListItemProperties"), ex.StackTrace);
            }
            return DocItemProp;
        }

        /// <summary>
        /// this method gets a URI to a site that lists the current user's followed documents.
        /// </summary>
        /// <param name="propSiteURL"></param>
        /// <returns></returns>
        public static string GetFollowedDocUrl(string propSiteURL)
        {
            string followedDocUri = string.Empty;
            try
            {
                using (SPSite site = new SPSite(propSiteURL))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPServiceContext serverContext = SPServiceContext.GetContext(web.Site);
                        UserProfileManager profileManager = new UserProfileManager(serverContext);
                        string userString = web.CurrentUser.LoginName.ToString();
                        UserProfile userProfile = profileManager.GetUserProfile(userString);
                        if (userProfile != null)
                        {
                            SPSocialFollowingManager manager = new SPSocialFollowingManager(userProfile);
                            SPSocialActorInfo actorInfo = new SPSocialActorInfo();
                            actorInfo.AccountName = web.CurrentUser.LoginName;
                            followedDocUri = manager.FollowedDocumentsUri.AbsoluteUri;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetFollowedDocUrl"), ex.StackTrace);
            }
            return followedDocUri;
        }

        #endregion

        #region News / Events and Announcement Section

        /// <summary>
        /// This method executes to search api to get most recent items from the current site
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="propListName"></param>
        /// <param name="errMSG"></param>
        /// <returns></returns>
        public static string GetRecentNewsFromCAMLQuery(string propListName, string propDisplayCount, string errMSG, string defaultImageUrl)
        {
            string mostRecent = string.Empty;
            try
            {
                //if (CacheHelper.IsIncache("Global_RecentNews"))
                //{
                //    mostRecent = (string)CacheHelper.GetFromCache("Global_RecentNews");
                //}
                //else
                // {

                List oList = clientContext.Web.Lists.GetByTitle(propListName);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query><RowLimit>" + propDisplayCount + "</RowLimit></View>";
                ListItemCollection items = oList.GetItems(camlQuery);
                clientContext.Load(items);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                clientContext.ExecuteQuery();
                mostRecent = GenerateRecentNewsHTML(items, errMSG, defaultImageUrl);
                //}
            }


            catch (Exception ex)
            {
                mostRecent = errMSG;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "CommonHelper", "GetRecentNewsFromCAMLQuery"), ex.StackTrace);
            }

            return mostRecent;
        }

        /// <summary>
        ///  This method generates dynamic html based on the list item collection.
        /// </summary>
        /// <param name="clientcontext"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string GenerateRecentNewsHTML(ListItemCollection oItems, string errMSG, string defaultImageUrl)
        {
            #region before integration
            /*
            string mostRecentHTML = string.Empty;
            StringBuilder oSb = new StringBuilder();
            StringBuilder corosalimage = new StringBuilder();
            StringBuilder corosalimage2 = new StringBuilder();
            StringBuilder final = new StringBuilder();

            try
            {
                corosalimage.Append(@"<div id='carousel' class='carousel slide' data-ride='carousel'><ol class='carousel-indicators carousel-indicators-numbers'>");
                int ccount = 0, ccountnext = 0;
                corosalimage2.Append("<div class='carousel-inner' role='listbox'>");
                foreach (ListItem oItem in oItems)
                {
                    ccountnext = ccount + 1;
                    switch (ccount)
                    {
                        case 0:
                            corosalimage.Append("<li data-target='#carousel' data-slide-to='" + ccount + "' class='active'>" + ccountnext + "</li>");
                            break;
                        default:
                            corosalimage.Append("<li data-target='#carousel' data-slide-to='" + ccount + "' class=''>" + ccountnext + "</li>");
                            break;
                    }

                    switch (ccount)
                    {
                        case 0:
                            corosalimage2.Append("<div class='item active'>");

                            break;
                        case 1:
                            corosalimage2.Append("<div class='item'>");

                            break;

                        default:
                            corosalimage2.Append("<div class='item'>");
                            break;
                    }
                    string sImageURL = string.Empty;
                    if (oItem["Image"] != null)
                    {
                        FieldUrlValue imgfield = (FieldUrlValue)oItem["Image"];

                        if (imgfield != null)
                            sImageURL = imgfield.Url;
                    }
                    corosalimage2.Append("<img src='" + sImageURL + "' alt='" + oItem["Title"] + "'>");
                 corosalimage2.Append("<div class='carousel-caption'></div>");
                 //   corosalimage2.Append("<div class='carousel-caption'>");
                    corosalimage2.Append("<div class=''>");
                    string sTitleUrl = oItem["FileDirRef"] + "/DispForm.aspx?ID=" + oItem["ID"];
                    corosalimage2.Append("<a href='" + sTitleUrl + "'>");
                    corosalimage2.Append("<h5 class='slide-txt'>" + oItem["Title"] + "</h5></a>");
                    corosalimage2.Append("<span class='glyphicon glyphicon-time time-slide' aria-hidden='true'></span>");
                    DateTime dt = (DateTime)oItem["Created"];
                    string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                    corosalimage2.Append("<span class='icon-txt'>" + formattedDate + "</span>");
                    corosalimage2.Append("</div></div>");
                  //  corosalimage2.Append("</div>");
                    ccount++;
                }

                corosalimage.Append("</ol>");
                oSb.Append("</div></div>");
                final.Append(corosalimage);
                final.Append(corosalimage2);
                final.Append(oSb);
                
                mostRecentHTML = final.ToString();
            
            }
            catch (Exception ex)
            {
                mostRecentHTML = errMSG;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "CommonHelper", "GenerateRecentNewsHTML"), ex.StackTrace);
            }
            return mostRecentHTML;
              */
            #endregion

            string mostRecentHTML = string.Empty;
            StringBuilder oSbFirst = new StringBuilder();
            StringBuilder oSbMiddle = new StringBuilder();
            StringBuilder oSbLast = new StringBuilder();
            StringBuilder oSbFinal = new StringBuilder();
            try
            {
                oSbMiddle.Append(@"<div id='carousel' class='carousel slide' data-ride='carousel'><ol class='carousel-indicators carousel-indicators-numbers'>");
                int ccount = 0, ccountnext = 0;
                oSbLast.Append("<div class='carousel-inner' role='listbox'>");
                foreach (ListItem oItem in oItems)
                {

                    ccountnext = ccount + 1;
                    switch (ccount)
                    {
                        case 0:
                            oSbMiddle.Append("<li data-target='#carousel' data-slide-to='" + ccount + "' class='active'>" + ccountnext + "</li>");
                            break;
                        default:
                            oSbMiddle.Append("<li data-target='#carousel' data-slide-to='" + ccount + "' class=''>" + ccountnext + "</li>");
                            break;
                    }

                    switch (ccount)
                    {
                        case 0:
                            oSbLast.Append("<div class='item active'>");
                            break;
                        case 1:
                            oSbLast.Append("<div class='item'>");
                            break;
                        default:
                            oSbLast.Append("<div class='item'>");
                            break;
                    }
                    string sImageURL = string.Empty;
                    if (oItem[Constants.IMAGE] != null)
                    {
                        FieldUrlValue imgfield = (FieldUrlValue)oItem[Constants.IMAGE];

                        if (imgfield != null)
                            sImageURL = imgfield.Url;
                    }
                    else sImageURL = defaultImageUrl;

                    string sTitleUrl = oItem[Constants.FILE__DIR_REF] + "/DispForm.aspx?ID=" + oItem[Constants.ID];
                    oSbLast.Append("<a href='" + sTitleUrl + "'>");
                    oSbLast.Append("<img src='" + sImageURL + "' ></a>");
                    oSbLast.Append("<div class='carousel-caption'></div>");
                    oSbLast.Append("<div class=''>");
                    oSbLast.Append("<a href='" + sTitleUrl + "'>");
                    oSbLast.Append("<h5 class='slide-txt'>" + oItem[Constants.TITLE] + "</h5></a>");
                    oSbLast.Append("<span class='glyphicon glyphicon-time time-slide' aria-hidden='true'></span>");
                    DateTime dt = (DateTime)oItem[Constants.MODIFIED];
                    string formattedDate = string.Format("{0:dd MMM yyyy}", dt);
                    oSbLast.Append("<span class='icon-txt'>" + formattedDate + "</span>");
                    oSbLast.Append("</div></div>");
                    ccount++;
                }

                oSbMiddle.Append("</ol>");
                oSbFirst.Append("</div></div>");
                oSbFinal.Append(oSbMiddle);
                oSbFinal.Append(oSbLast);
                oSbFinal.Append(oSbFirst);
                if (ccount > 0)
                {
                    //CacheHelper.SaveTocache("Global_RecentNews", oSbFinal.ToString(), DateTime.Now.AddSeconds(60.00));
                    mostRecentHTML = oSbFinal.ToString();
                }
                else
                {
                    mostRecentHTML = Constants.NO_DATA_ERRMSG;
                }

            }
            catch (Exception ex)
            {
                mostRecentHTML = errMSG;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "CommonHelper", "GenerateRecentNewsHTML"), ex.StackTrace);
            }
            return mostRecentHTML;

        }

        #endregion

        #region Department Best practice section

        /// <summary>
        /// This method executes to search api to get the document icon for best practices section
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string LoadIconRecent(ClientContext clientContext, string fileName)
        {
            Web oWeb = clientContext.Web;
            ClientResult<string> icon = oWeb.MapToIcon(fileName, string.Empty, Microsoft.SharePoint.Client.Utilities.IconSize.Size16);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            clientContext.ExecuteQuery();
            string iconURL = string.Empty;

            if (oWeb.Context.Url.EndsWith("/"))
            {
                return oWeb.Context.Url + "_layouts/images/" + icon.Value;
            }
            else
            {
                return oWeb.Context.Url + "/_layouts/images/" + icon.Value;
            }

        }

        /// <summary>
        /// Get the workflow details of list.
        /// </summary>        
        /// <returns></returns>
        public static Dictionary<string, string> GetListWorkflowDetails(ClientContext clientContext, string propListName)
        {
            Dictionary<string, string> listDetails = new Dictionary<string, string>();
            string WorkflowFieldName = string.Empty;
            bool isWorkflowAttached = false;
            bool enableModeration = false;
            try
            {
                //List<ListDetails> listDetails = new List<ListDetails>();
                List oList = clientContext.Web.Lists.GetByTitle(propListName);
                clientContext.Load(oList, list => list.WorkflowAssociations, list => list.Fields, list => list.EnableModeration);
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                clientContext.ExecuteQuery();
                WorkflowFieldName = string.Empty;
                isWorkflowAttached = false;
                enableModeration = oList.EnableModeration;

                if (oList.WorkflowAssociations.Count > 0)
                {
                    foreach (WorkflowAssociation workflow in oList.WorkflowAssociations)
                    {
                        if (workflow.BaseId.Equals(Constants.approvalWorkflowBaseId))
                        {
                            isWorkflowAttached = true;
                            Field field = oList.Fields.GetByInternalNameOrTitle(workflow.Name);
                            clientContext.Load(field);
                            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            clientContext.ExecuteQuery();
                            WorkflowFieldName = field.InternalName;
                        }
                    }
                }
                //listDetails.WorkflowFieldName = WorkflowFieldName;
                listDetails.Add(Constants.IsWorkflowAttached, isWorkflowAttached.ToString());
                listDetails.Add(Constants.IsModerationEnabled, enableModeration.ToString());
                listDetails.Add(Constants.WorkflowFieldName, WorkflowFieldName);
            }
            catch (Exception)
            {
                listDetails.Add(Constants.IsWorkflowAttached, isWorkflowAttached.ToString());
                listDetails.Add(Constants.IsModerationEnabled, enableModeration.ToString());
                listDetails.Add(Constants.WorkflowFieldName, WorkflowFieldName);
            }

            return listDetails;
        }

        /// <summary>
        /// Check whether the workflow status is approved or not. If Yes, show this item on page.
        /// </summary>        
        /// <returns></returns>
        public static bool IsAddItem(bool isWorkflowAttached, bool enableModeration, ListItem item, string fieldName, string userName, List<string> userGroups, bool isUserSiteAdmin)
        {
            int itemWorkflowStatus = 0, itemApprovalStatus = -1;
            bool addNewItem = false;

            if (isUserSiteAdmin)
            {
                addNewItem = true;
            }
            else if (userGroups != null)
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    foreach (RoleAssignment role in item.RoleAssignments)
                    {
                        if (userGroups.Contains(role.Member.Title) || userGroups.Contains(role.Member.LoginName))
                        {
                            addNewItem = true;
                            break;
                        }
                    }
                }
            }

            if (addNewItem)
            {
                if (isWorkflowAttached)
                {
                    if (item[fieldName] != null)
                    {
                        itemWorkflowStatus = Convert.ToInt32(item[fieldName].ToString());
                    }
                    if (itemWorkflowStatus == 16 || itemWorkflowStatus == 5 || itemWorkflowStatus == 0)
                    {
                        addNewItem = true;
                    }
                    else
                    {
                        addNewItem = false;
                    }
                }
                else
                {
                    if (enableModeration)
                    {
                        if (item[Constants.ApprovalStatus] != null)
                        {
                            itemApprovalStatus = Convert.ToInt32(item[Constants.ApprovalStatus].ToString());
                        }
                        if (itemApprovalStatus == 0 || itemApprovalStatus == -1)
                        {
                            addNewItem = true;
                        }
                        else
                        {
                            addNewItem = false;
                        }
                    }
                    else
                    {
                        addNewItem = true;
                    }
                }
            }
            return addNewItem;
        }


        /// <summary>
        /// Check whether the workflow status is approved or not. If Yes, show this item on page.
        /// </summary>        
        /// <returns></returns>
        public static bool IsAddItem(bool isWorkflowAttached, bool enableModeration, ListItem item, string fieldName, string userName, List<string> userGroups)
        {
            int itemWorkflowStatus = 0, itemApprovalStatus = -1;
            bool addNewItem = false;

            if (userGroups != null)
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    foreach (RoleAssignment role in item.RoleAssignments)
                    {
                        if (userGroups.Contains(role.Member.Title) || userGroups.Contains(role.Member.LoginName))
                        {
                            addNewItem = true;
                            break;
                        }
                    }
                }
            }

            if (addNewItem)
            {
                if (isWorkflowAttached)
                {
                    if (item[fieldName] != null)
                    {
                        itemWorkflowStatus = Convert.ToInt32(item[fieldName].ToString());
                    }
                    if (itemWorkflowStatus == 16 || itemWorkflowStatus == 5 || itemWorkflowStatus == 0)
                    {
                        addNewItem = true;
                    }
                    else
                    {
                        addNewItem = false;
                    }
                }
                else
                {
                    if (enableModeration)
                    {
                        if (item[Constants.ApprovalStatus] != null)
                        {
                            itemApprovalStatus = Convert.ToInt32(item[Constants.ApprovalStatus].ToString());
                        }
                        if (itemApprovalStatus == 0 || itemApprovalStatus == -1)
                        {
                            addNewItem = true;
                        }
                        else
                        {
                            addNewItem = false;
                        }
                    }
                    else
                    {
                        addNewItem = true;
                    }
                }
            }
            return addNewItem;
        }


        /// <summary>
        /// Get the groups for which user belongs to in the site
        /// </summary>       
        /// <returns></returns>
        public static List<string> GetUserGroups(ClientContext clientContext, GroupCollection groups, string loggedInUsername)
        {
            List<string> userGroupArray = new List<string>();
            foreach (var group in groups)
            {
                clientContext.Load(group, g => g.Users.Where(user => user.LoginName == loggedInUsername));
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                clientContext.ExecuteQuery();

                if (group.Users.Count > 0)
                {
                    userGroupArray.Add(group.Title);
                }
            }

            return userGroupArray;
        }

        /// <summary>
        /// Check whether user is site Admin
        /// </summary>       
        /// <returns></returns>
        public static bool IsUserSiteAdmin(ClientContext clientContext, string userName)
        {
            Web web = clientContext.Web;
            UserCollection userCollection = web.SiteUsers;
            clientContext.Load(userCollection);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            clientContext.ExecuteQuery();
            foreach (User user in userCollection)
            {
                if (user.LoginName.Equals(userName))
                {
                    if (user.IsSiteAdmin)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Decode the user name
        /// </summary>
        /// <returns></returns>
        public static string DecodeUserName(string userName)
        {
            string loggedInUsername = string.Empty;
            if (userName.Contains("\\\\"))
            {
                loggedInUsername = userName.Replace("\\\\", "\\");
            }
            return loggedInUsername;
        }

        /// <summary>
        /// This method executes to search api to get the document icon for Onboarding section section
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string LoadIconOnboard(string fileName)
        {

            Web oWeb = clientContext.Web;
            ClientResult<string> icon = oWeb.MapToIcon(fileName, string.Empty, Microsoft.SharePoint.Client.Utilities.IconSize.Size16);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            clientContext.ExecuteQuery();
            return oWeb.Context.Url + "_layouts/images/" + icon.Value;

        }


        #endregion

        #endregion

        #region SSOM Section

        #region Common Section

        /// <summary>
        /// This method loads the clientcontext and check whether the list exsists in the current context or not.
        /// </summary>
        /// <returns></returns>
        public static bool CheckListExists(string propListName, string propItemCount, string propSiteUrl, string propErrMsg)
        {
            bool isExists = false;
            try
            {
                using (SPSite site = new SPSite(propSiteUrl))
                {
                    using (SPWeb oWeb = site.OpenWeb())
                    {
                        SPList list = oWeb.Lists.TryGetList(propListName);
                        if (list != null)
                        {
                            isExists = true;
                        }
                        else
                        {
                            isExists = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "CheckListExists"), errMessage);
            }
            return isExists;
        }


        /// <summary>
        /// This method returns list sever relative url.
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public static string[] GetListDetails(string propListName, string propSiteUrl)
        {
            string[] listURL = new string[3];
            try
            {

                using (SPSite site = new SPSite(propSiteUrl))
                {
                    using (SPWeb oWeb = site.OpenWeb())
                    {
                        SPList oList = oWeb.Lists.TryGetList(propListName);
                        listURL[0] = oList.RootFolder.ServerRelativeUrl;
                        listURL[1] = oList.RootFolder.ServerRelativeUrl + "/" + Constants.NEWFORM;
                        string webURL = string.Empty;

                        webURL = (!oWeb.Url.EndsWith("/")) ? oWeb.Url + "/" : oWeb.Url;
                        string listUrl = string.Empty;
                        if (oList.BaseType == SPBaseType.DocumentLibrary)
                            listUrl = webURL + oList.RootFolder.Name;
                        else
                            listUrl = webURL + "Lists/" + oList.RootFolder.Name;
                        listURL[2] = webURL + "_layouts/15/Upload.aspx?List={" + oList.ID + "}&RootFolder=&Source=" + listUrl + "";
                    }
                }

            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetListDetails"), errMessage);
            }

            return listURL;
        }


        /// <summary>
        /// Check whether user is site Admin
        /// </summary>       
        /// <returns></returns>
        public static bool IsUserSiteAdministrator(string propSiteUrl, string userName)
        {
            bool isValid = false;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(propSiteUrl))
                    {
                        using (SPWeb oWeb = site.OpenWeb())
                        {
                            SPUserCollection userCollection = oWeb.SiteUsers;
                            foreach (SPUser user in userCollection)
                            {
                                if (user.LoginName.Equals(userName))
                                {
                                    if (user.IsSiteAdmin)
                                    {
                                        isValid = true;
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "IsUserSiteAdministrator"), errMessage);
            }
            return isValid;
        }

        /// <summary>
        /// Get the groups for which user belongs to in the site
        /// </summary>       
        /// <returns></returns>
        public static List<string> GetUserGroupArray(string propSiteUrl)
        {
            List<string> userGroupArray = new List<string>();
            try
            {
                using (SPSite site = new SPSite(propSiteUrl))
                {
                    using (SPWeb oWeb = site.OpenWeb())
                    {
                        //Retreiving the current logged in user
                        SPUser currentUser = oWeb.CurrentUser;
                        //Retrieving all the user groups in the site/web
                        SPGroupCollection userGroups = currentUser.Groups;
                        foreach (SPGroup group in userGroups)
                        {
                            if (group.Users.Count > 0)
                            {
                                userGroupArray.Add(group.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetUserGroupArray"), errMessage);
            }
            return userGroupArray;
        }

        /// <summary>
        /// Get the workflow details of list.
        /// </summary>        
        /// <returns></returns>
        public static Dictionary<string, string> GetWorkflowDetails(string propSiteUrl, string propListName)
        {
            Dictionary<string, string> listDetails = new Dictionary<string, string>();
            string WorkflowFieldName = string.Empty;
            bool isWorkflowAttached = false;
            bool enableModeration = false;
            try
            {
                using (SPSite site = new SPSite(propSiteUrl))
                {
                    using (SPWeb oWeb = site.OpenWeb())
                    {
                        SPList oList = oWeb.Lists.TryGetList(propListName);
                        WorkflowFieldName = string.Empty;
                        isWorkflowAttached = false;
                        enableModeration = oList.EnableModeration;
                        if (oList.WorkflowAssociations.Count > 0)
                        {
                            foreach (SPWorkflowAssociation workflow in oList.WorkflowAssociations)
                            {
                                if (workflow.BaseId.Equals(Constants.approvalWorkflowBaseId))
                                {
                                    isWorkflowAttached = true;
                                    SPField field = oList.Fields.GetFieldByInternalName(workflow.Name);
                                    WorkflowFieldName = field.InternalName;
                                }
                            }
                        }
                    }
                }
                listDetails.Add(Constants.IsWorkflowAttached, isWorkflowAttached.ToString());
                listDetails.Add(Constants.IsModerationEnabled, enableModeration.ToString());
                listDetails.Add(Constants.WorkflowFieldName, WorkflowFieldName);
            }
            catch (Exception ex)
            {
                listDetails.Add(Constants.IsWorkflowAttached, isWorkflowAttached.ToString());
                listDetails.Add(Constants.IsModerationEnabled, enableModeration.ToString());
                listDetails.Add(Constants.WorkflowFieldName, WorkflowFieldName);

                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetWorkflowDetails"), errMessage);

            }

            return listDetails;
        }

        ///// <summary>
        ///// Check whether the workflow status is approved or not. If Yes, show this item on page.
        ///// </summary>        
        ///// <returns></returns>
        public static bool IsAddItemToList(bool isWorkflowAttached, bool enableModeration, SPListItem item, string fieldName, bool isUserSiteAdmin)
        {
            int itemWorkflowStatus = 0, itemApprovalStatus = -1;
            bool addNewItem = false;
            try
            {
                if (isUserSiteAdmin)
                {
                    addNewItem = true;
                }

                if (isWorkflowAttached)
                {
                    if (item[fieldName] != null)
                    {
                        itemWorkflowStatus = Convert.ToInt32(item[fieldName].ToString());
                    }
                    if (itemWorkflowStatus == 16 || itemWorkflowStatus == 5 || itemWorkflowStatus == 0)
                    {
                        addNewItem = true;
                    }
                    else
                    {
                        addNewItem = false;
                    }
                }
                else
                {
                    if (enableModeration)
                    {
                        if (item[Constants.ApprovalStatus] != null)
                        {
                            itemApprovalStatus = Convert.ToInt32(item[Constants.ApprovalStatus].ToString());
                        }
                        if (itemApprovalStatus == 0 || itemApprovalStatus == -1)
                        {
                            addNewItem = true;
                        }
                        else
                        {
                            addNewItem = false;
                        }
                    }
                    else
                    {
                        addNewItem = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "IsAddItemToList"), errMessage);
            }

            return addNewItem;
        }


        public static SPUser GetSPUser(SPListItem item, string key)
        {
            SPUser user = null;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    SPFieldUserValue userValue = new SPFieldUserValue(item.Web, item[key].ToString());
                    if (userValue != null)
                    {
                        user = userValue.User;
                    }
                });
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "GetSPUser"), errMessage);
            }
            return user;
        }

        /// <summary>
        /// This method executes to search api to get the document icon for best practices section
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string LoadRecentIcon(string propSiteUrl, string fileName)
        {
            string iconURL = string.Empty;
            string icon = string.Empty;
            try
            {
                using (SPSite site = new SPSite(propSiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        icon = SPUtility.MapToIcon(web, fileName, string.Empty, IconSize.Size16);

                        if (web.Url.EndsWith("/"))
                        {
                            return web.Url + "_layouts/images/" + icon;
                        }
                        else
                        {
                            return web.Url + "/_layouts/images/" + icon;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "LoadRecentIcon"), errMessage);
            }
            return string.Empty;
        }

        /// <summary>
        /// check if user has permission to view th list item
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listname"></param>
        /// <returns></returns>
        public static Boolean DoesUserHaveSitePermissions(this SPWeb web, String listname)
        {
            Boolean perm = false;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (var elevSite = new SPSite(web.Url))
                    {
                        using (var elevWeb = elevSite.OpenWeb())
                        {
                            SPList list = elevWeb.Lists[listname];
                            perm = list.DoesUserHavePermissions(web.CurrentUser,
                        SPBasePermissions.ViewListItems);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message + " : " + ex.StackTrace;
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "KMPortalException-CommonHelper", "DoesUserHaveSitePermissions"), errMessage);
            }
            return perm;
        }

        #endregion

        #endregion



    }
}


