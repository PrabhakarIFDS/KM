using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFDS.KMPortal.KnowledgeBaseJob;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.ServiceModel;
using IFDS.KMPortal.KnowledgeBaseJob.KnowledgeBaseService;
using System.IO;
using System.Configuration;
namespace IFDS.KMPortal.KnowledgeBaseJob
{
    class Program
    {
        
        static void Main(string[] args)
        {

            try
            {
                #region Knowledge Base
               // Render("StartTime", DateTime.Now.ToLongTimeString());
                KBServiceClient oKBServiceClient = new KBServiceClient();

                oKBServiceClient.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["sUserName"];
                oKBServiceClient.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["sPassword"];

                string sUserName = ConfigurationManager.AppSettings["sUserName"];
                string sPassword = ConfigurationManager.AppSettings["sPassword"];

                KBUserSession oKBUserSession = new KBUserSession();
                Random r = new Random();
                oKBUserSession.SessionID = "api " + r.Next().ToString();
                int clientid = Convert.ToInt32(ConfigurationManager.AppSettings["ClientID"]);
                int portalid = Convert.ToInt32(ConfigurationManager.AppSettings["PortalID"]); 
               

                Render("Version", oKBServiceClient.GetCurrentAPIVersion());
                PortalProperties portalprop = oKBServiceClient.GetPortalByPortalId(clientid, portalid);
               
                UserMember oUserMember = oKBServiceClient.GetUserByUserInfo(sUserName, sPassword, UserType.ExternalUser, oKBUserSession, clientid, portalid);
              


                SearchTypeMember[] SearchTypeMember = oKBServiceClient.GetSearchOptionList(portalprop, oUserMember);
                SearchTypeMember[] oSearchTypeMember = oKBServiceClient.GetSearchOptionList(portalprop, oUserMember);
                FAQMember[] fAQM = oKBServiceClient.GetFAQs(portalprop, oUserMember);
              

                #region SolutionFinders


               //  Solution Finders Table 
                DataTable dtSolutionFinders = new DataTable();

                dtSolutionFinders.Columns.Add(new DataColumn(Constants.SOLUTION_FINDER_ID, Type.GetType("System.Int32")));

                dtSolutionFinders.Columns.Add(new DataColumn(Constants.SOLUTION_FINDER_NAME, Type.GetType("System.String")));
                dtSolutionFinders.Columns.Add(new DataColumn(Constants.SOLUTION_FINDER_STATEMENT, Type.GetType("System.String")));
                dtSolutionFinders.Columns.Add(new DataColumn(Constants.SOLUTION_FINDER_CHOICEID, Type.GetType("System.Int32")));
                dtSolutionFinders.Columns.Add(new DataColumn(Constants.SOLUTION_FINDER_CHOICE_NAME, Type.GetType("System.String")));




                #region SolutionFinder Articles
                DataTable dtSolutionFinderArticles = new DataTable();

                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.ARTICLE_ID, Type.GetType("System.Int32")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.ARTICLE_NAME, Type.GetType("System.String")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.ARTICLE_VIEWED, Type.GetType("System.String")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.ARTICLE_SUMMARY, Type.GetType("System.String")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.REVIEWED, Type.GetType("System.DateTime")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.EXTENSION, Type.GetType("System.String")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.ATTACHMENTS, Type.GetType("System.String")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.ARTICLE_RELATED, Type.GetType("System.String")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.CREATED_DATE, Type.GetType("System.DateTime")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.ARTICLE_CONTENT, Type.GetType("System.String")));
                dtSolutionFinderArticles.Columns.Add(new DataColumn(Constants.ARTICLE_URL, Type.GetType("System.String")));
                SolutionFinderMember[] oSolutionFinders = oKBServiceClient.GetSolutionFinders(portalprop, oUserMember);

                FetchSolutionFinders(oKBServiceClient, portalprop, oUserMember, ref dtSolutionFinders, ref dtSolutionFinderArticles, oSolutionFinders, false);
                BulkInsert(dtSolutionFinders, Constants.MOXIE_SOLUTION_FINDERS, true);
                BulkInsert(dtSolutionFinderArticles, Constants.MOXIE_ARTICLES, true);


                #endregion


                #endregion

                #region  KB Articles
                PortalKBMember[] oPortalKBMembers = oKBServiceClient.GetKnowledgebasePortalByPortalId(portalprop, oUserMember);
                Render("Message","Entering in to Portal KB Members");
                foreach (PortalKBMember oPortalKBMember in oPortalKBMembers)
                {

                    CategoryMember[] oCategoryMembers = oKBServiceClient.GetCategoryByKBID(oPortalKBMember.KBId, portalprop, oUserMember);

                    // FAQCategoryMember[] oFAQMember = oMoxieClient.GetFAQCategoriesByKBId(oPortalKBMember.KBId, portalprop, oUserMember);
                    DataTable dtKBrticles = new DataTable();

                    dtKBrticles.Columns.Add(new DataColumn(Constants.ARTICLE_ID, Type.GetType("System.Int32")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.ARTICLE_NAME, Type.GetType("System.String")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.ARTICLE_VIEWED, Type.GetType("System.String")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.ARTICLE_SUMMARY, Type.GetType("System.String")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.REVIEWED, Type.GetType("System.DateTime")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.EXTENSION, Type.GetType("System.String")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.ATTACHMENTS, Type.GetType("System.String")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.ARTICLE_RELATED, Type.GetType("System.String")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.CREATED_DATE, Type.GetType("System.DateTime")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.ARTICLE_CONTENT, Type.GetType("System.String")));
                    dtKBrticles.Columns.Add(new DataColumn(Constants.ARTICLE_URL, Type.GetType("System.String")));

                    ReadCategoryArticles(oKBServiceClient, portalprop, oUserMember, oCategoryMembers, ref dtKBrticles);

                    BulkInsert(dtKBrticles, Constants.MOXIE_ARTICLES, false);

                  

                }




                #endregion




                #region GlossaryItems

                DataTable dtGloseryTerms = new DataTable();

                dtGloseryTerms.Columns.Add(new DataColumn(Constants.ID, Type.GetType("System.Int32")));

                dtGloseryTerms.Columns.Add(new DataColumn(Constants.TERM, Type.GetType("System.String")));
                dtGloseryTerms.Columns.Add(new DataColumn(Constants.TEXT, Type.GetType("System.String")));

                GlossaryMember[] GM = oKBServiceClient.GetGlossaries(portalprop, oUserMember);

                foreach (GlossaryMember ogm in GM)
                {
                    
                    GlossaryTermMember[] oGlossaryTerms = oKBServiceClient.GetGlossaryTerms(ogm.GlossaryID, portalprop, oUserMember); // 794 Glossary Terms

                    foreach (GlossaryTermMember oGlossaryTerm in oGlossaryTerms)
                    {
                        DataRow drGloseryTerm = dtGloseryTerms.NewRow();
                        drGloseryTerm[Constants.ID] = oGlossaryTerm.Id;
                        drGloseryTerm[Constants.TERM] = oGlossaryTerm.Term;
                        drGloseryTerm[Constants.TEXT] = oGlossaryTerm.Text;
                        // dr["Text"] = oGlossaryTerm.ExtensionData; 
                        dtGloseryTerms.Rows.Add(drGloseryTerm);

                    }
                }

                BulkInsert(dtGloseryTerms, Constants.MOXIE_GLOSSARY_TERMS, true);
                #endregion


                #endregion

                Console.WriteLine("Succesfully Completed");
                //Console.WriteLine("Press enter to quit");
                //Console.Read();

            }
            catch (Exception ex)
            {
                Render("Error", ex.ToString());
                LogError(ex);
               
            }


        }
        public static void LogError(Exception ex)
        {
            SqlConnection conn = GetSQLConnection();
            try
            {

                conn.Open();
                DateTime dt = DateTime.Now;
                SqlCommand cmd = new SqlCommand("Insert into DWR_Z_IFDSLogs values ('" + ex.ToString() + "','" + dt + "')", conn);
                cmd.ExecuteNonQuery();
            }
            catch
            {
              //DO Nothing  
            }
            finally
            {

                conn.Close();
            }
        }
        private static void FetchSolutionFinders(KBServiceClient oMoxieClient, PortalProperties portalprop, UserMember oUserMember, ref DataTable dtSolutionFinders, ref DataTable dtSolutionFinderArticles, SolutionFinderMember[] oSolutionFinders, bool IsSolutionFinderChoice)
        {
            foreach (SolutionFinderMember oSolutionFinder in oSolutionFinders)
            {

                DataRow drsolutionFinder = dtSolutionFinders.NewRow();
                drsolutionFinder[Constants.SOLUTION_FINDER_ID] = oSolutionFinder.SolutionFinderID;
                drsolutionFinder[Constants.SOLUTION_FINDER_NAME] = oSolutionFinder.SolutionFinderName;
                drsolutionFinder[Constants.SOLUTION_FINDER_STATEMENT] = oSolutionFinder.SFstatement;
                drsolutionFinder[Constants.SOLUTION_FINDER_CHOICEID] = oSolutionFinder.SFChoiceID;
                drsolutionFinder[Constants.SOLUTION_FINDER_CHOICE_NAME] = oSolutionFinder.SFChoiceName;
                dtSolutionFinders.Rows.Add(drsolutionFinder);



                ArticleMember[] oSolutionFinderArticalMembers;
                SolutionFinderMember[] oSolutionFinderMemberChoices;
                if (IsSolutionFinderChoice)
                {
                    oSolutionFinderArticalMembers = oMoxieClient.GetSFArticlesByChoiceID(oSolutionFinder.SFChoiceID, portalprop, oUserMember);

                }
                else
                {


                    oSolutionFinderArticalMembers = oMoxieClient.GetSFArticlesBySFID(oSolutionFinder.SolutionFinderID, portalprop, oUserMember);

                }

                ReadArticles(oMoxieClient, portalprop, oUserMember, ref dtSolutionFinderArticles, oSolutionFinderArticalMembers);
                if (oSolutionFinder.IsParent)
                {
                    oSolutionFinderMemberChoices = oMoxieClient.GetSolutionFinderChoicesByParentID(oSolutionFinder.SFChoiceID, portalprop, oUserMember);
                }
                else
                {
                    oSolutionFinderMemberChoices = oMoxieClient.GetSolutionFinderChoicesBySFID(oSolutionFinder.SolutionFinderID, portalprop, oUserMember);
                }

                if (oSolutionFinderMemberChoices.Length > 0)
                {
                    FetchSolutionFinders(oMoxieClient, portalprop, oUserMember, ref dtSolutionFinders, ref dtSolutionFinderArticles, oSolutionFinderMemberChoices, true);
                }


            }

        }

        private static void ReadCategoryArticles(KBServiceClient oMoxieClient, PortalProperties portalprop, UserMember oUserMember, CategoryMember[] oCategoryMembers, ref DataTable dtArticles)
        {
            foreach (CategoryMember oCategoryMember in oCategoryMembers)
            {
               

                ArticleMember[] oArticleMembers = oMoxieClient.GetArticleByCatID(oCategoryMember.CategoryID, portalprop, oUserMember);

                ReadArticles(oMoxieClient, portalprop, oUserMember, ref dtArticles, oArticleMembers);
                CategoryMember[] oSubCategoryMembers = oMoxieClient.GetSubCategoryByCatID(oCategoryMember.CategoryID, portalprop, oUserMember);
                if (oSubCategoryMembers.Length > 0)
                {
                    ReadCategoryArticles(oMoxieClient, portalprop, oUserMember, oSubCategoryMembers, ref dtArticles);
                }
               

            }

        }



        private static void ReadArticles(KBServiceClient oMoxieClient, PortalProperties portalprop, UserMember oUserMember, ref DataTable dtArticles, ArticleMember[] oArticleMembers)
        {
            foreach (ArticleMember oArticleMember in oArticleMembers)
            {
               
                DataRow drArticles = dtArticles.NewRow();
                drArticles[Constants.ARTICLE_ID] = oArticleMember.ArticleID;
                drArticles[Constants.ARTICLE_NAME] = oArticleMember.ArticleName;

                ArticleInfoMember oArticleInfoMember = oMoxieClient.GetArticleInfo(oArticleMember.ArticleID, portalprop, oUserMember);
                drArticles[Constants.ARTICLE_VIEWED] = oArticleInfoMember.ArticleView;
                string sArticleSummary = oMoxieClient.GetArticleSummary(oArticleMember.ArticleID, portalprop, oUserMember);
                string sArticleUrl = oMoxieClient.GetArticleViewSecureURL(oArticleMember.ArticleID, portalprop, oUserMember);
                sArticleUrl = sArticleUrl.Substring(0, sArticleUrl.LastIndexOf("&source=Article")) + "&source=Article";
                ArticleResourceMember[] oArticleResourceMember = oMoxieClient.GetArticleResourceByArticleID(oArticleMember.ArticleID, portalprop, oUserMember);

                ArticleInfoProperties oArticleInfoProperties = oMoxieClient.GetArticleMetadata(oArticleMember.ArticleID, portalprop, oUserMember);
                drArticles[Constants.REVIEWED] = oArticleInfoProperties.LastReviewedDate == DateTime.MinValue ? (DateTime)SqlDateTime.MinValue : oArticleInfoProperties.LastReviewedDate;
                drArticles[Constants.EXTENSION] = oArticleInfoProperties.ArticleFileExtension;
                Attachment[] oAttachments = oArticleInfoProperties.ArticleAttachments;
                string sAttachments = string.Empty;
                string ArticleContent = string.Empty;

                using (var sMStream = oMoxieClient.GetArticleContent(oArticleMember.ArticleID, string.Empty, portalprop, oUserMember))
                {

                    var sr = new StreamReader(sMStream);
                    ArticleContent = sr.ReadToEnd();

                }
                drArticles[Constants.ARTICLE_CONTENT] = ArticleContent;
                foreach (Attachment oAttachment in oAttachments)
                {
                    sAttachments += oAttachment.FileName;
                    sAttachments += ",";

                }
                drArticles[Constants.ATTACHMENTS] = sAttachments;
                ArticleRelated[] oArticlesRelated = oArticleInfoProperties.RelatedArticle;


                string sRelatedArticle = string.Empty;
                foreach (ArticleRelated oArticleRelated in oArticlesRelated)
                {
                    sRelatedArticle += oArticleRelated.ArticleName;
                    sRelatedArticle += ",";

                }
                drArticles[Constants.ARTICLE_RELATED] = sRelatedArticle;


                drArticles[Constants.ARTICLE_SUMMARY] = sArticleSummary;
                drArticles[Constants.CREATED_DATE] = oArticleMember.CreateDate == DateTime.MinValue ? (DateTime)SqlDateTime.MinValue : oArticleMember.CreateDate;
                drArticles[Constants.ARTICLE_URL] = sArticleUrl;
                dtArticles.Rows.Add(drArticles);
              
            }
        }


        public static bool BulkInsert(DataTable dt, string tableName, bool delete)
        {
            bool isSuccuss;
            SqlConnection SqlConnectionObj = GetSQLConnection();
            try
            {

                

                SqlConnectionObj.Open();
                if (delete)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "delete from " + tableName;
                    cmd.Connection = SqlConnectionObj;
                    int rowsAffected = cmd.ExecuteNonQuery();
                }
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(SqlConnectionObj, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null))
                {
                    bulkCopy.DestinationTableName = tableName;

                    bulkCopy.BulkCopyTimeout = 0;

                    if (tableName.Equals(Constants.MOXIE_GLOSSARY_TERMS))
                    {
                        bulkCopy.ColumnMappings.Add(Constants.ID, Constants.ID);
                        bulkCopy.ColumnMappings.Add(Constants.TERM, Constants.TERM);
                        bulkCopy.ColumnMappings.Add(Constants.TEXT, Constants.TEXT);
                    }
                    else if (tableName.Equals(Constants.MOXIE_ARTICLES))
                    {
                        bulkCopy.ColumnMappings.Add(Constants.ARTICLE_ID, Constants.ARTICLE_ID);
                        bulkCopy.ColumnMappings.Add(Constants.ARTICLE_NAME, Constants.ARTICLE_NAME);
                        bulkCopy.ColumnMappings.Add(Constants.ARTICLE_VIEWED, Constants.ARTICLE_VIEWED);
                        bulkCopy.ColumnMappings.Add(Constants.ARTICLE_SUMMARY, Constants.ARTICLE_SUMMARY);
                        bulkCopy.ColumnMappings.Add(Constants.REVIEWED, Constants.REVIEWED);
                        bulkCopy.ColumnMappings.Add(Constants.EXTENSION, Constants.EXTENSION);
                        bulkCopy.ColumnMappings.Add(Constants.ATTACHMENTS, Constants.ATTACHMENTS);
                        bulkCopy.ColumnMappings.Add(Constants.ARTICLE_RELATED, Constants.ARTICLE_RELATED);
                        bulkCopy.ColumnMappings.Add(Constants.CREATED_DATE, Constants.CREATED_DATE);
                        bulkCopy.ColumnMappings.Add(Constants.ARTICLE_CONTENT, Constants.ARTICLE_CONTENT);
                        bulkCopy.ColumnMappings.Add(Constants.ARTICLE_URL, Constants.ARTICLE_URL);
                    }
                    else{

                        bulkCopy.ColumnMappings.Add(Constants.SOLUTION_FINDER_ID, Constants.SOLUTION_FINDER_ID);
                        bulkCopy.ColumnMappings.Add(Constants.SOLUTION_FINDER_STATEMENT, Constants.SOLUTION_FINDER_STATEMENT);
                        bulkCopy.ColumnMappings.Add(Constants.SOLUTION_FINDER_CHOICEID, Constants.SOLUTION_FINDER_CHOICEID);
                        bulkCopy.ColumnMappings.Add(Constants.SOLUTION_FINDER_CHOICE_NAME, Constants.SOLUTION_FINDER_CHOICE_NAME);
                    }

                    bulkCopy.WriteToServer(dt);
                    isSuccuss = true;
                    Render("Migrated ", tableName);
                }

            }
            catch (Exception ex)
            {
                isSuccuss = false;
                Render("Error", ex.Message);
                LogError(ex);
            }
            finally
            {
                SqlConnectionObj.Close();
                SqlConnectionObj.Dispose();
            }
            return isSuccuss;
        }

        private static SqlConnection GetSQLConnection()
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            SqlConnection con = new SqlConnection(connectionString);
            return con;
        }
        private static void Render(string sText, string sValue)
        {

            Console.WriteLine(string.Concat(sText, ":", sValue));

        }
    }
}
