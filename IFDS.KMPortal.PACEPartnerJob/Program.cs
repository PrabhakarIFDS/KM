using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFDS.KMPortal.PACEPartnerJob.PACEPartnerService;
using IFDS.KMPortal.PACEPartnerJob.PACEPartnerServiceDetails;
using System.Configuration;
using System.Text.RegularExpressions; 
namespace IFDS.KMPortal.PACEPartnerJob
{
    class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                
                DataTable dtCourseCatalog = new DataTable();

                dtCourseCatalog.Columns.Add(new DataColumn(Constants.TITLE, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.CREATED_DATE, Type.GetType("System.DateTime")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.DESCRIPTION, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.SUBJECT_IDS, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.SUBJECT_TITLES, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.PRICE, Type.GetType("System.Decimal")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.PROVIDER_NAME, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.TYPE, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.DEEP_LINK_URL, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.COURSE_DURATION, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.OU_AVAILABILITY, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.LO_TITLES, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.LO_INSTRUCTIONS, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.LO_LOCATIONS, Type.GetType("System.String")));
                dtCourseCatalog.Columns.Add(new DataColumn(Constants.LO_INSTRUCTORS, Type.GetType("System.String")));
                //  dtCourseCatalog.Columns.Add(new DataColumn("SystemType", Type.GetType("System.String")));
                ReadData(PACEPartnerService.CatalogType.CURRICULUM, ref dtCourseCatalog);
                ReadData(PACEPartnerService.CatalogType.EVENT, ref dtCourseCatalog);
                ReadData(PACEPartnerService.CatalogType.LIBRARY, ref dtCourseCatalog);
                ReadData(PACEPartnerService.CatalogType.MATERIAL, ref dtCourseCatalog);
                ReadData(PACEPartnerService.CatalogType.ONLINE, ref dtCourseCatalog);
                ReadData(PACEPartnerService.CatalogType.POSTING, ref dtCourseCatalog);
                ReadData(PACEPartnerService.CatalogType.QUICKCOURSE, ref dtCourseCatalog);
                ReadData(PACEPartnerService.CatalogType.SESSION, ref dtCourseCatalog);
                ReadData(PACEPartnerService.CatalogType.TEST, ref dtCourseCatalog);
                BulkInsert(dtCourseCatalog, "CNR_Z_CourseCatalog");
                Console.WriteLine("Succesfully Completed");
              
            }
            catch(Exception ex)
            {

                Console.WriteLine(ex.ToString());
                Console.Read();
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
                 SqlCommand cmd = new SqlCommand("Insert into DWR_Z_IFDSLogs values ('" + ex.ToString() + "','" + dt + "')",conn);
                 cmd.ExecuteNonQuery();
             }
             catch
             {
                 conn.Close(); 
             }
        }

        public static void ReadData(PACEPartnerService.CatalogType catalogtype, ref DataTable dtCourseCatalog)
        {
            CatalogwebserviceSoapClient client = new CatalogwebserviceSoapClient();

            client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["UserName"];
            client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["Password"]; 
            IFDS.KMPortal.PACEPartnerJob.PACEPartnerService.AuthHeader authHeader = new IFDS.KMPortal.PACEPartnerJob.PACEPartnerService.AuthHeader();
            authHeader.CorpName = ConfigurationManager.AppSettings["CorpName"];
            authHeader.UserId = ConfigurationManager.AppSettings["UserId"];
            authHeader.Signature = ConfigurationManager.AppSettings["Signature"]; 
            int total;
            int pageCounter = 0;
            CatalogSearchQuery query;
            query = new CatalogSearchQuery();
            query.Type = catalogtype;
            query.PageNum = 0;
            CatalogSearchResultMessage results;
            results = client.Search(authHeader, query);
            if (results != null)
            {
                total = results.Results.Count();
                while (total > 0)
                {
                    for (int i = 0; i < total; i++)
                    {
                        CatalogSearchResult result;
                        result = results.Results[i];


                        DataRow dr = dtCourseCatalog.NewRow();
                        dr[Constants.TITLE] = result.CourseTitle;
                        dr[Constants.CREATED_DATE] = Convert.ToDateTime(result.CreatedDate);
                        dr[Constants.DESCRIPTION] = result.Descr;
                        dr[Constants.COURSE_DURATION] = result.Duration;
                        //CornerStoneIntegration.CornerStoneClient.Field[] fields = result.Fields;
                        //for (int j = 0; j < fields.Count(); j++)
                        //{
                        //    Console.WriteLine("Field Title: " + j.ToString() + fields[j].Title);
                        //    Console.WriteLine("Field Type: " + j.ToString() + fields[j].Type);
                        //    Console.WriteLine("Field Value: " + j.ToString() + fields[j].Value);
                        //}
                        LoSubject[] subjects = result.LoSubjectList;
                        string subjectIDs = string.Empty;
                        string subjectTitles = string.Empty;
                        for (int k = 0; k < subjects.Count(); k++)
                        {
                            IFDS.KMPortal.PACEPartnerJob.PACEPartnerService.Subject[] subjecthierarchy = subjects[k].SubjectWithHierarchy;
                            for (int l = 0; l < subjecthierarchy.Count(); l++)
                            {
                                subjectIDs += subjecthierarchy[l].Id;
                                subjectTitles += subjecthierarchy[l].Title;
                                // if (l != subjecthierarchy.Count() - 1)
                                //{
                                subjectIDs += ",";
                                subjectTitles += ",";
                                // }
                                //Console.WriteLine("Subject Parent Title: " + l.ToString() + subjecthierarchy[l].ParentTitle);
                                // Console.WriteLine("Subject Title: " + l.ToString() + subjecthierarchy[l].Title);
                            }
                        }
                        if (subjectIDs != string.Empty)
                            subjectIDs = subjectIDs.Substring(0, subjectIDs.Length - 1);
                        if (subjectTitles != string.Empty)
                            subjectTitles = subjectTitles.Substring(0, subjectTitles.Length - 1);
                        dr[Constants.SUBJECT_IDS] = subjectIDs;
                        dr[Constants.SUBJECT_TITLES] = subjectTitles;
                        dr[Constants.PRICE] = result.Price;
                        dr[Constants.PROVIDER_NAME] = result.ProviderName;
                        dr[Constants.TYPE] = result.Type;
                        string availabilityIDs = string.Empty;
                        string availabilitytypes = string.Empty;
                        Availability[] availabilities = result.Availabilities;
                        for (int m = 0; m < availabilities.Count(); m++)
                        {
                            try
                            {
                                OUAvailability ouavailaiblity = (OUAvailability)availabilities[m];
                                availabilityIDs += availabilities[m].Id;
                                availabilityIDs += ",";
                            }
                            catch (Exception ex)
                            {

                            }

                            // availabilityIDs += ",";
                            //Console.WriteLine("Availability  Subs: " + m.ToString() + availabilities[m].IncludeSubs);
                            //availabilities[m].
                        }
                        if (availabilityIDs != string.Empty)
                            availabilityIDs = availabilityIDs.Substring(0, availabilityIDs.Length - 1);
                        dr[Constants.OU_AVAILABILITY] = availabilityIDs;
                        // dr["OUAvailabilityTypes"] = availabilitytypes;
                        //result.
                        //Console.WriteLine("Object : " + result.ObjectId);
                        GetDetailsWebServiceSoapClient clientdetails = new GetDetailsWebServiceSoapClient();
                        clientdetails.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["UserName"];
                        clientdetails.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["Password"];
                        IFDS.KMPortal.PACEPartnerJob.PACEPartnerServiceDetails.AuthHeader authHeaderDetails = new IFDS.KMPortal.PACEPartnerJob.PACEPartnerServiceDetails.AuthHeader();
                        authHeaderDetails.CorpName = ConfigurationManager.AppSettings["CorpName"];
                        authHeaderDetails.UserId = ConfigurationManager.AppSettings["UserId"];
                        authHeaderDetails.Signature = ConfigurationManager.AppSettings["Signature"];
                        // CatalogResult resultdetails = clientdetails.GetDetails(authHeaderDetails, result.ObjectId);
                        switch (result.Type)
                        {
                            case PACEPartnerService.CatalogType.CURRICULUM:
                                CatalogResultCurriculum curriculumdetails = (CatalogResultCurriculum)clientdetails.GetDetails(authHeaderDetails, result.ObjectId);
                                dr[Constants.DEEP_LINK_URL] = ReplaceObjectId(curriculumdetails.DeepLinkURL, curriculumdetails.ObjectId);
                                SectionDetail[] sectiondetails = curriculumdetails.SectionDetails;
                                string sectiontitle = string.Empty;
                                string sectionInstruction = string.Empty;
                                for (int counteri = 0; counteri < sectiondetails.Count(); counteri++)
                                {
                                    SectionDetail sectiondetail = sectiondetails[counteri];
                                    sectiontitle += sectiondetail.SectionTitle;
                                    sectiontitle += ",";
                                    sectionInstruction += sectiondetail.SectionInstructions;
                                    sectionInstruction += ",";
                                }
                                if (sectiontitle != string.Empty)
                                {
                                    sectiontitle = sectiontitle.Substring(0, sectiontitle.Length - 1);
                                    dr[Constants.LO_TITLES] = sectiontitle;
                                }
                                if (sectionInstruction != string.Empty)
                                {
                                    sectionInstruction = sectionInstruction.Substring(0, sectionInstruction.Length - 1);
                                    dr[Constants.LO_INSTRUCTIONS] = sectionInstruction;
                                }
                                break;
                            case PACEPartnerService.CatalogType.EVENT:
                                CatalogResultEvent eventdetails = (CatalogResultEvent)clientdetails.GetDetails(authHeaderDetails, result.ObjectId);
                                dr[Constants.DEEP_LINK_URL] = ReplaceObjectId(eventdetails.DeepLinkURL, eventdetails.ObjectId);
                                SessionDetails[] sessiondetails = eventdetails.Sessions;
                                string sessiontitle = string.Empty;
                                string sessionLocation = string.Empty;
                                for (int counterj = 0; counterj < sessiondetails.Count(); counterj++)
                                {
                                    SessionDetails sessiondetail = sessiondetails[counterj];
                                    //sessiontitle += sessiondetail.;
                                    // sessiontitle += ",";
                                    sessionLocation += sessiondetail.SessionLocation;
                                    sessionLocation += ",";
                                }
                                //if (sessiontitle != string.Empty)
                                //{
                                //    sessiontitle = sessiontitle.Substring(0, sessiontitle.Length - 1);
                                //    dr["LOTitles"] = sessiontitle;
                                //}
                                if (sessionLocation != string.Empty)
                                {
                                    sessionLocation = sessionLocation.Substring(0, sessionLocation.Length - 1);
                                    dr[Constants.LO_LOCATIONS] = sessionLocation;
                                }
                                // eventdetails.
                                break;
                            case PACEPartnerService.CatalogType.MATERIAL:
                                CatalogResultMaterial materialdetails = (CatalogResultMaterial)clientdetails.GetDetails(authHeaderDetails, result.ObjectId);
                                dr[Constants.DEEP_LINK_URL] = ReplaceObjectId(materialdetails.DeepLinkURL, materialdetails.ObjectId);
                                string mattype = materialdetails.MaterialType;
                                //materialdetails.
                                break;
                            case PACEPartnerService.CatalogType.ONLINE:
                                CatalogResultOnlineCourse onlinedetails = (CatalogResultOnlineCourse)clientdetails.GetDetails(authHeaderDetails, result.ObjectId);
                                if (onlinedetails != null)
                                    dr[Constants.DEEP_LINK_URL] = ReplaceObjectId(onlinedetails.DeepLinkURL, onlinedetails.ObjectId);

                                break;
                            case PACEPartnerService.CatalogType.SESSION:
                                CatalogResultSession coursesessiondetails = (CatalogResultSession)clientdetails.GetDetails(authHeaderDetails, result.ObjectId);
                                dr[Constants.DEEP_LINK_URL] = ReplaceObjectId(coursesessiondetails.DeepLinkURL, coursesessiondetails.ObjectId);
                                PartDetails[] partdetails = coursesessiondetails.Parts;
                                //string partTitle = string.Empty;
                                string partLocation = string.Empty;
                                string partInstructor = string.Empty;
                                for (int counterk = 0; counterk < partdetails.Count(); counterk++)
                                {
                                    PartDetails partdetail = partdetails[counterk];
                                    partLocation += partdetail.PartLocation;
                                    partLocation += ",";
                                    partInstructor += partdetail.PartInstructor;
                                    partInstructor += ",";
                                }
                                if (partLocation != string.Empty)
                                {
                                    partLocation = partLocation.Substring(0, partLocation.Length - 1);
                                    dr[Constants.LO_LOCATIONS] = partLocation;
                                }
                                if (partInstructor != string.Empty)
                                {
                                    partInstructor = partInstructor.Substring(0, partInstructor.Length - 1);
                                    dr[Constants.LO_INSTRUCTORS] = partInstructor;
                                }
                                break;
                            case PACEPartnerService.CatalogType.TEST:
                                CatalogResultTest testdetails = (CatalogResultTest)clientdetails.GetDetails(authHeaderDetails, result.ObjectId);
                                dr[Constants.DEEP_LINK_URL] = ReplaceObjectId(testdetails.DeepLinkURL, testdetails.ObjectId);

                                break;
                            default:
                                CatalogResult resultdetails = clientdetails.GetDetails(authHeaderDetails, result.ObjectId);
                                if (resultdetails != null)
                                    dr[Constants.DEEP_LINK_URL] = ReplaceObjectId(resultdetails.DeepLinkURL, resultdetails.ObjectId); ;
                                break;
                        }


                        // dr["SystemType"] = "PACEPartner";
                        dtCourseCatalog.Rows.Add(dr);
                        //Console.WriteLine("Provider Name: " + result.ProviderName);
                        // Console.WriteLine("Type: " + result.Type);
                    }

                    pageCounter++;

                    query = new CatalogSearchQuery();
                    query.Type = catalogtype;
                    query.PageNum = pageCounter;
                    results = client.Search(authHeader, query);
                    total = results.Results.Count();
                }
            }
            else
            {
                Console.WriteLine("Resulsts are Empty. Please check the User Permissions");
                Console.Read();
                LogError(new Exception("Resulsts are Empty. Please check the User Permissions"));
            }

        }

        public static bool BulkInsert(DataTable dt, string tableName)
        {
            bool isSuccuss;
            SqlConnection SqlConnectionObj = GetSQLConnection();
            try
            {

                SqlConnectionObj.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "delete from " + tableName;
                cmd.Connection = SqlConnectionObj;
                int rowsAffected = cmd.ExecuteNonQuery();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(SqlConnectionObj, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null))
                {

                    
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.ColumnMappings.Add(Constants.TITLE, Constants.TITLE);
                    bulkCopy.ColumnMappings.Add(Constants.CREATED_DATE,Constants.CREATED_DATE);

                    bulkCopy.ColumnMappings.Add(Constants.DESCRIPTION, Constants.DESCRIPTION);
                    bulkCopy.ColumnMappings.Add(Constants.SUBJECT_IDS, Constants.SUBJECT_IDS);
                    bulkCopy.ColumnMappings.Add(Constants.PRICE, Constants.PRICE);
                    bulkCopy.ColumnMappings.Add(Constants.PROVIDER_NAME, Constants.PROVIDER_NAME);
                    bulkCopy.ColumnMappings.Add(Constants.TYPE, Constants.TYPE);
                    bulkCopy.ColumnMappings.Add(Constants.DEEP_LINK_URL, Constants.DEEP_LINK_URL);
                    bulkCopy.ColumnMappings.Add(Constants.COURSE_DURATION, Constants.COURSE_DURATION);
                    bulkCopy.ColumnMappings.Add(Constants.OU_AVAILABILITY, Constants.OU_AVAILABILITY);
                    bulkCopy.ColumnMappings.Add(Constants.LO_TITLES, Constants.LO_TITLES);

                    bulkCopy.ColumnMappings.Add(Constants.LO_INSTRUCTIONS, Constants.LO_INSTRUCTIONS);
                    bulkCopy.ColumnMappings.Add(Constants.LO_LOCATIONS, Constants.LO_LOCATIONS);
                    bulkCopy.ColumnMappings.Add(Constants.LO_INSTRUCTORS, Constants.LO_INSTRUCTORS);
                        
                       
                    bulkCopy.WriteToServer(dt);
                    isSuccuss = true;
                }
            }
            catch (Exception ex)
            {
                isSuccuss = false;
                Console.WriteLine(ex.ToString());
                Console.Read();
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
            string connectionString = ConfigurationManager.AppSettings["connectionString"]; ;
            SqlConnection con = new SqlConnection(connectionString);
            return con;
        }

        private static string ReplaceObjectId(string DeepLinkURL,string ObjectId)
        {
            // FIX for the PRODuction DeepLinkURL
            string PACEPartnerURL = ConfigurationManager.AppSettings["PACEPartnerLOIDURL"];

            string replacedString = PACEPartnerURL + "?loId=" + ObjectId;
           // string actualHref = string.Empty;
           // string regex = "href=['']([^'']+?)['']";
           // Match match = Regex.Match(DeepLinkURL, regex);
           // if (match.Success)
           // {
           //     actualHref = match.Groups[1].Value;
           // }
           //  string replacedString  =string.Empty;
           //  if (!string.IsNullOrEmpty(actualHref))
           //  {
           //      replacedString = actualHref.Split('?')[0] + "?loId=" + ObjectId;
           //  }
           //  else
           //  {
           //      replacedString = DeepLinkURL;
           //  }
           //// string replacedString = Regex.Replace(DeepLinkURL, @"(?<=href=').+?(?='\s)", replaceObjectId);
            return replacedString;
        }

    }
}
