using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using Microsoft.SharePoint;

namespace Global_TopContriibutors
{

    #region CSOM
    //public class DepartmentListMaster
    //{
    //    public string Department { get; set; }
    //    public string siteurl { get; set; }
    //}

    //public class TopContributorsGlobal
    //{
    //    public FieldUserValue Member { get; set; }
    //    public int MemberStatus { get; set; }
    //    public string GiftedBadge { get; set; }
    //    public int Replies { get; set; }
    //    public int BestReplies { get; set; }
    //    public int Discussions { get; set; }
    //    public int ReputationScore { get; set; }
    //}

    //class Program
    //{

    //    public static List<DepartmentListMaster> GetAllDepartmentsList(string siteurl, string DepartmentMasterListname)
    //    {
    //        List<DepartmentListMaster> dplm = new List<DepartmentListMaster>();

    //        try
    //        {
    //            using (ClientContext clientContext = new ClientContext(siteurl))
    //            {
    //                Web site = clientContext.Web;
    //                List oList = site.Lists.GetByTitle(DepartmentMasterListname);
    //                CamlQuery camlQuery = new CamlQuery();
    //                camlQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Title' Ascending='True' /></OrderBy><ViewFields><FieldRef Name='Title'/><FieldRef Name='SiteURL'/><FieldRef Name='Department Name'/></ViewFields></Query></View>";
    //                ListItemCollection collListItem = oList.GetItems(camlQuery);
    //                clientContext.Load(collListItem);
    //                clientContext.ExecuteQuery();

    //                if (collListItem.Count > 0)
    //                {
    //                    foreach (ListItem eachdepartment in collListItem)
    //                    {
    //                        DepartmentListMaster listitem = new DepartmentListMaster();
    //                        listitem.Department = Convert.ToString(eachdepartment["Title"]);
    //                        listitem.siteurl = Convert.ToString(eachdepartment["SiteURL"]);
    //                        dplm.Add(listitem);
    //                    }
    //                }


    //            }



    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine("Top Contributors and Learner webpart -- Error querying external list -" + DepartmentMasterListname.ToString() + ex.StackTrace.ToString());

    //        }

    //        return dplm;

    //    }

    //    public static void ResetvaluesinTopContributorsGlobal(string siteurl, string TopContributorsGlobalListname)
    //    {
    //        try
    //        {
    //            using (ClientContext clientContext = new ClientContext(siteurl))
    //            {
    //                Web site = clientContext.Web;
    //                List oList = site.Lists.GetByTitle(TopContributorsGlobalListname);
    //                CamlQuery camlQuery = new CamlQuery();
    //                camlQuery.ViewXml = "<View><Query><Where><IsNotNull><FieldRef Name='Member' /></IsNotNull></Where></Query></View>";
    //                ListItemCollection collListItem = oList.GetItems(camlQuery);
    //                clientContext.Load(collListItem);
    //                clientContext.ExecuteQuery();

    //                if (collListItem.Count > 0)
    //                {
    //                    foreach (ListItem oListItem in collListItem)
    //                    {
    //                        oListItem["NumberOfReplies"] = 0;
    //                        oListItem["NumberOfBestResponses"] = 0;
    //                        oListItem["NumberOfDiscussions"] = 0;
    //                        oListItem["ReputationScore"] = 0;
    //                        oListItem.Update();

    //                        clientContext.ExecuteQuery();
    //                    }
    //                }


    //            }



    //        }
    //        catch (Exception)
    //        {
    //            //Console.WriteLine("Top Contributors and Learner webpart -- Error querying external list -" + DepartmentMasterListname.ToString() + ex.StackTrace.ToString());

    //        }



    //    }


    //    static void Main(string[] args)
    //    {
    //        string dusername, username, siteurl, TCGlobalListname, KMDeptMasterListname;
    //        try
    //        {
    //            dusername = ConfigurationManager.AppSettings["Username"];
    //            username = ConfigurationManager.AppSettings["Password"];
    //            siteurl = ConfigurationManager.AppSettings["SiteURL"];
    //            TCGlobalListname = ConfigurationManager.AppSettings["TopContributors-GlobalList"];
    //            KMDeptMasterListname = ConfigurationManager.AppSettings["KMDepartmentMasterList"];

    //            //Console.WriteLine(dusername);
    //            //Console.WriteLine(username);
    //            //Console.WriteLine(siteurl);
    //            //Console.WriteLine(TCGlobalListname);
    //            //Console.WriteLine(KMDeptMasterListname);


    //            ClientContext clientContext = new ClientContext(siteurl);
    //            List oList = clientContext.Web.Lists.GetByTitle(TCGlobalListname);

    //            List<TopContributorsGlobal> tpglobal = new List<TopContributorsGlobal>();
    //            List<DepartmentListMaster> dplist = new List<DepartmentListMaster>();
    //            dplist = GetAllDepartmentsList(siteurl, KMDeptMasterListname);
    //            ResetvaluesinTopContributorsGlobal(siteurl, TCGlobalListname);

    //            foreach (DepartmentListMaster eachdept in dplist)
    //            {
    //                //Console.WriteLine(eachdept.Department);
    //                //Console.WriteLine(eachdept.siteurl);



    //                ClientContext childcontext = new ClientContext(eachdept.siteurl);

    //                Web web = childcontext.Web;
    //                List eachList = web.Lists.GetByTitle("Community Members");
    //                CamlQuery camlQuery2 = new CamlQuery();
    //                camlQuery2.ViewXml = "<View><Query><Where><IsNotNull><FieldRef Name='Member' /></IsNotNull></Where><ViewFields><FieldRef Name='Member'/><FieldRef Name='MemberStatusInt'/><FieldRef Name='NumberOfBestResponses'/><FieldRef Name='NumberOfDiscussions'/><FieldRef Name='NumberOfReplies'/><FieldRef Name='ReputationScore'/><FieldRef Name='GiftedBadgeText'/></ViewFields></Query></View>";
    //                ListItemCollection collListItem2 = eachList.GetItems(camlQuery2);
    //                childcontext.Load(collListItem2);
    //                childcontext.ExecuteQuery();
    //                if (collListItem2.Count > 0)
    //                {
    //                    foreach (ListItem eachdepartment in collListItem2)
    //                    {
    //                        TopContributorsGlobal listitem = new TopContributorsGlobal();
    //                        listitem.Member = (FieldUserValue)eachdepartment["Member"];
    //                        listitem.GiftedBadge = Convert.ToString(eachdepartment["GiftedBadgeText"]);
    //                        listitem.Replies = Convert.ToInt32(eachdepartment["NumberOfReplies"]);
    //                        listitem.BestReplies = Convert.ToInt32(eachdepartment["NumberOfBestResponses"]);
    //                        listitem.ReputationScore = Convert.ToInt32(eachdepartment["ReputationScore"]);
    //                        listitem.Discussions = Convert.ToInt32(eachdepartment["NumberOfDiscussions"]);
    //                        tpglobal.Add(listitem);
    //                    }


    //                }



    //                foreach (TopContributorsGlobal tpeach in tpglobal)
    //                {
    //                    CamlQuery camlQueryone = new CamlQuery();
    //                    camlQueryone.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Member' /><Value Type='User'>" + tpeach.Member.LookupValue + "</Value></Eq></Where></Query></View>";
    //                    ListItemCollection collListItem1 = oList.GetItems(camlQueryone);
    //                    clientContext.Load(collListItem1);
    //                    clientContext.ExecuteQuery();

    //                    if (collListItem1.Count == 1)
    //                    {
    //                        ListItem oListItem = collListItem1.Cast<ListItem>().FirstOrDefault();

    //                        oListItem["GiftedBadgeText"] = tpeach.GiftedBadge;
    //                        oListItem["NumberOfReplies"] = Convert.ToInt32(oListItem["NumberOfReplies"]) + tpeach.Replies;
    //                        oListItem["NumberOfBestResponses"] = Convert.ToInt32(oListItem["NumberOfBestResponses"]) + tpeach.BestReplies;
    //                        oListItem["NumberOfDiscussions"] = Convert.ToInt32(oListItem["NumberOfDiscussions"]) + tpeach.Discussions;
    //                        oListItem["ReputationScore"] = Convert.ToInt32(oListItem["ReputationScore"]) + tpeach.ReputationScore;
    //                        oListItem.Update();

    //                        clientContext.ExecuteQuery();
    //                    }

    //                    if (collListItem1.Count <= 0)
    //                    {
    //                        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
    //                        ListItem oListItem = oList.AddItem(itemCreateInfo);

    //                        oListItem["Member"] = (FieldUserValue)tpeach.Member;
    //                        oListItem["GiftedBadgeText"] = tpeach.GiftedBadge;
    //                        oListItem["NumberOfReplies"] = tpeach.Replies;
    //                        oListItem["NumberOfBestResponses"] = tpeach.BestReplies;
    //                        oListItem["NumberOfDiscussions"] = tpeach.Discussions;
    //                        oListItem["ReputationScore"] = tpeach.ReputationScore;

    //                        oListItem.Update();
    //                        clientContext.ExecuteQuery();
    //                    }



    //                }



    //            }

    //            //Console.WriteLine("Finished processing");
    //            //Console.ReadKey();



    //        }
    //        catch (Exception)
    //        {

    //            //Console.WriteLine(ex.StackTrace.ToString());
    //        }

    //        //Console.ReadKey();
    //    }
    //}
    #endregion

    #region SSOM

    public class DepartmentListMaster
    {
        public string Department { get; set; }
        public string siteurl { get; set; }
    }

    public class TopContributorsGlobal
    {
        public SPFieldUserValue Member { get; set; }
        public int MemberStatus { get; set; }
        public string GiftedBadge { get; set; }
        public int Replies { get; set; }
        public int BestReplies { get; set; }
        public int Discussions { get; set; }
        public int ReputationScore { get; set; }
    }

    class Program
    {

        public static List<DepartmentListMaster> GetAllDepartmentsList(string siteurl, string DepartmentMasterListname)
        {
            List<DepartmentListMaster> dplm = new List<DepartmentListMaster>();

            try
            {
                using (SPSite site = new SPSite(siteurl))
                {
                    using (SPWeb Web = site.OpenWeb())
                    {
                        SPList oList = Web.Lists.TryGetList(DepartmentMasterListname);
                        SPQuery oQuery = new SPQuery();
                        oQuery.ViewXml = "<View><Query><OrderBy><FieldRef Name='Title' Ascending='True' /></OrderBy><ViewFields><FieldRef Name='Title'/><FieldRef Name='SiteURL'/><FieldRef Name='Department Name'/></ViewFields></Query></View>";
                        SPListItemCollection collListItem = oList.GetItems(oQuery);
                        if (collListItem.Count > 0)
                        {
                            foreach (SPListItem eachdepartment in collListItem)
                            {
                                DepartmentListMaster listitem = new DepartmentListMaster();
                                listitem.Department = Convert.ToString(eachdepartment["Title"]);
                                listitem.siteurl = Convert.ToString(eachdepartment["SiteURL"]);
                                dplm.Add(listitem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Top contributor's Remote timer Job -- GetAllDepartmentsList -" + DepartmentMasterListname.ToString() + ex.StackTrace.ToString());
                Console.ReadLine();

            }

            return dplm;

        }

        public static void ResetvaluesinTopContributorsGlobal(string siteurl, string TopContributorsGlobalListname)
        {
            try
            {
                using (SPSite site = new SPSite(siteurl))
                {
                    using (SPWeb Web = site.OpenWeb())
                    {
                        SPList oList = Web.Lists.TryGetList(TopContributorsGlobalListname);
                        SPQuery oQuery = new SPQuery();
                        oQuery.ViewXml = "<View><Query><Where><IsNotNull><FieldRef Name='Member' /></IsNotNull></Where></Query></View>";
                        SPListItemCollection collListItem = oList.GetItems(oQuery);
                        if (collListItem.Count > 0)
                        {
                            foreach (SPListItem oListItem in collListItem)
                            {
                                oListItem["NumberOfReplies"] = 0;
                                oListItem["NumberOfBestResponses"] = 0;
                                oListItem["NumberOfDiscussions"] = 0;
                                oListItem["ReputationScore"] = 0;
                                oListItem.Update();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Top contributor's Remote timer Job -- ResetvaluesinTopContributorsGlobal -" + TopContributorsGlobalListname.ToString() + ex.StackTrace.ToString());
                Console.ReadLine();
            }
        }

        public static SPFieldUserValue GetSPUser(SPListItem item, string key)
        {
            SPUser user = null;
            SPFieldUserValue userValue = new SPFieldUserValue(item.Web, item[key].ToString());
            if (userValue != null)
            {
                user = userValue.User;
            }
            return userValue;
        }

        static void Main(string[] args)
        {
            string dusername, username, siteurl, TCGlobalListname, KMDeptMasterListname;
            try
            {
                dusername = ConfigurationManager.AppSettings["Username"];
                username = ConfigurationManager.AppSettings["Password"];
                siteurl = ConfigurationManager.AppSettings["SiteURL"];
                TCGlobalListname = ConfigurationManager.AppSettings["TopContributors-GlobalList"];
                KMDeptMasterListname = ConfigurationManager.AppSettings["KMDepartmentMasterList"];

                List<TopContributorsGlobal> tpglobal = new List<TopContributorsGlobal>();
                List<DepartmentListMaster> dplist = new List<DepartmentListMaster>();

                using (SPSite site = new SPSite(siteurl))
                {
                    using (SPWeb Web = site.OpenWeb())
                    {
                        SPList oList = Web.Lists.TryGetList(TCGlobalListname);
                        dplist = GetAllDepartmentsList(siteurl, KMDeptMasterListname);
                        ResetvaluesinTopContributorsGlobal(siteurl, TCGlobalListname);
                        foreach (DepartmentListMaster eachdept in dplist)
                        {
                            using (SPSite osite = new SPSite(eachdept.siteurl))
                            {
                                Console.WriteLine("Reading Site Url : " + eachdept.siteurl);
                                using (SPWeb oWeb = site.OpenWeb())
                                {
                                    SPList sList = oWeb.Lists.TryGetList("Community Members");
                                    SPQuery oQuery = new SPQuery();
                                    oQuery.ViewXml = "<View><Query><Where><IsNotNull><FieldRef Name='Member' /></IsNotNull></Where><ViewFields><FieldRef Name='Member'/><FieldRef Name='MemberStatusInt'/><FieldRef Name='NumberOfBestResponses'/><FieldRef Name='NumberOfDiscussions'/><FieldRef Name='NumberOfReplies'/><FieldRef Name='ReputationScore'/><FieldRef Name='GiftedBadgeText'/></ViewFields></Query></View>";
                                    SPListItemCollection items = sList.GetItems(oQuery);
                                    if (items.Count > 0)
                                    {
                                        foreach (SPListItem eachdepartment in items)
                                        {
                                            TopContributorsGlobal listitem = new TopContributorsGlobal();
                                            listitem.Member = GetSPUser(eachdepartment, "Member");
                                            listitem.GiftedBadge = Convert.ToString(eachdepartment["GiftedBadgeText"]);
                                            listitem.Replies = Convert.ToInt32(eachdepartment["NumberOfReplies"]);
                                            listitem.BestReplies = Convert.ToInt32(eachdepartment["NumberOfBestResponses"]);
                                            listitem.ReputationScore = Convert.ToInt32(eachdepartment["ReputationScore"]);
                                            listitem.Discussions = Convert.ToInt32(eachdepartment["NumberOfDiscussions"]);
                                            tpglobal.Add(listitem);
                                        }
                                    }
                                }
                            }
                        }

                        foreach (TopContributorsGlobal tpeach in tpglobal)
                        {
                            SPQuery oQuery = new SPQuery();
                            oQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Member' /><Value Type='User'>" + tpeach.Member.LookupValue + "</Value></Eq></Where></Query></View>";
                            SPListItemCollection itemsTopContrib = oList.GetItems(oQuery);
                            if (itemsTopContrib.Count == 1)
                            {
                                SPListItem oListItem = itemsTopContrib.Cast<SPListItem>().FirstOrDefault();
                                oListItem["GiftedBadgeText"] = tpeach.GiftedBadge;
                                oListItem["NumberOfReplies"] = Convert.ToInt32(oListItem["NumberOfReplies"]) + tpeach.Replies;
                                oListItem["NumberOfBestResponses"] = Convert.ToInt32(oListItem["NumberOfBestResponses"]) + tpeach.BestReplies;
                                oListItem["NumberOfDiscussions"] = Convert.ToInt32(oListItem["NumberOfDiscussions"]) + tpeach.Discussions;
                                oListItem["ReputationScore"] = Convert.ToInt32(oListItem["ReputationScore"]) + tpeach.ReputationScore;
                                oListItem.Update();
                            }
                            if (itemsTopContrib.Count <= 0)
                            {
                                SPListItem oListItem = oList.Items.Add();
                                //oListItem["Member"] = (SPFieldUserValue)tpeach.Member;
                                SPFieldUserValue userValue = new SPFieldUserValue();
                                oListItem["Member"] = tpeach.Member;
                                oListItem["GiftedBadgeText"] = tpeach.GiftedBadge;
                                oListItem["NumberOfReplies"] = tpeach.Replies;
                                oListItem["NumberOfBestResponses"] = tpeach.BestReplies;
                                oListItem["NumberOfDiscussions"] = tpeach.Discussions;
                                oListItem["ReputationScore"] = tpeach.ReputationScore;
                                oListItem.Update();
                            }

                        }

                    }
                }

                Console.WriteLine("Completed Successfully");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Top contributor's Remote timer Job -- Main Method -" + ex.StackTrace.ToString());
                Console.ReadLine();
            }
        }
    }

    #endregion
}
