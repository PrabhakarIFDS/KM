using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace IFDS.KMPortal.ServiceNow
{
    class Program
    {
        static void Main(string[] args)
        {
            int counter = 1;
            string type = string.Empty;
            while (counter < 5)
            {
                switch (counter)
                {
                    case 1:
                        type = Constants.INCIDENTS;
                        break;
                    case 2:
                        type = Constants.PROBLEMS;
                        break;
                    case 3:
                        type = Constants.REQUESTS;
                        break;
                    case 4:
                        type = Constants.CHANGE_REQUESTS;
                        break;
                    default:
                        break;
                }
                DataTable dt = getDataFromExportFeed(type);
                BulkInsert(dt, "Snow_" + type);
                Console.WriteLine(type + " have been  migrated");
                counter++;
            }

            Console.WriteLine("Press enter to quit");
            Console.Read();
        }

        public static DataTable getDataFromExportFeed(string type)
        {
            //
            int read;
            string tempfilePath = @"c:\temp\" + type + ".xml";
            string feedURL = string.Empty;
            DataTable xmlDataTable = new DataTable();
            if (System.IO.File.Exists(tempfilePath))
                System.IO.File.Delete(tempfilePath);
            switch (type)
            {
                case "Incidents":
                    feedURL = "https://ifdsgrouptest.service-now.com/incident_list.do?XML&sysparm_order_by=number&sysparm_record_count=10000";
                    xmlDataTable.Columns.Add(new DataColumn(Constants.NUMBER, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.TITLE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.DESCRIPTION, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.STATE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.RESOULTION, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.CLOSE_CODE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.SYSID, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.CATEGORY, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.EXTERNAL_URL, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.ACTIVE, Type.GetType("System.Boolean")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.CREATED_DATE, Type.GetType("System.DateTime")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.MODIFIED_DATE, Type.GetType("System.DateTime")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.TYPE, Type.GetType("System.String")));
                    break;
                case "Problems":
                    feedURL = "https://ifdsgrouptest.service-now.com/problem_list.do?XML&sysparm_order_by=number&sysparm_record_count=10000";
                    xmlDataTable.Columns.Add(new DataColumn(Constants.NUMBER, Type.GetType("System.String")));
                    //xmlDataTable.Columns.Add(new DataColumn(Constants.TITLE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.DESCRIPTION, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.STATE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.RESOULTION, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.CLOSE_CODE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.SYSID, Type.GetType("System.String")));
                    //xmlDataTable.Columns.Add(new DataColumn(Constants.CATEGORY, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.EXTERNAL_URL, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.ACTIVE, Type.GetType("System.Boolean")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.CREATED_DATE, Type.GetType("System.DateTime")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.MODIFIED_DATE, Type.GetType("System.DateTime")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.TYPE, Type.GetType("System.String")));
                    break;
                case "Requests":
                    feedURL = "https://ifdsgrouptest.service-now.com/sc_request_list.do?XML&sysparm_order_by=number&sysparm_record_count=10000";
                    xmlDataTable.Columns.Add(new DataColumn(Constants.NUMBER, Type.GetType("System.String")));
                    //xmlDataTable.Columns.Add(new DataColumn(Constants.TITLE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.DESCRIPTION, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.STATE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.RESOULTION, Type.GetType("System.String")));
                    //xmlDataTable.Columns.Add(new DataColumn(Constants.CLOSE_CODE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.SYSID, Type.GetType("System.String")));
                    // xmlDataTable.Columns.Add(new DataColumn(Constants.CATEGORY, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.EXTERNAL_URL, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.ACTIVE, Type.GetType("System.Boolean")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.CREATED_DATE, Type.GetType("System.DateTime")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.MODIFIED_DATE, Type.GetType("System.DateTime")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.TYPE, Type.GetType("System.String")));
                    break;
                case "ChangeRequests":
                    feedURL = "https://ifdsgrouptest.service-now.com/change_request_list.do?XML&sysparm_order_by=number&sysparm_record_count=10000";
                    xmlDataTable.Columns.Add(new DataColumn(Constants.NUMBER, Type.GetType("System.String")));
                    //xmlDataTable.Columns.Add(new DataColumn(Constants.TITLE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.DESCRIPTION, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.STATE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.RESOULTION, Type.GetType("System.String")));
                    //xmlDataTable.Columns.Add(new DataColumn(Constants.CLOSE_CODE, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.SYSID, Type.GetType("System.String")));
                    // xmlDataTable.Columns.Add(new DataColumn(Constants.CATEGORY, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.EXTERNAL_URL, Type.GetType("System.String")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.ACTIVE, Type.GetType("System.Boolean")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.CREATED_DATE, Type.GetType("System.DateTime")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.MODIFIED_DATE, Type.GetType("System.DateTime")));
                    xmlDataTable.Columns.Add(new DataColumn(Constants.TYPE, Type.GetType("System.String")));
                    break;
                default:
                    break;
            }
            read = DownloadFile(feedURL, tempfilePath);



            string lastNumber;
            lastNumber = XmlToDataTable(tempfilePath, ref xmlDataTable, type);
            if (System.IO.File.Exists(tempfilePath))
                System.IO.File.Delete(tempfilePath);

            while (lastNumber != "")
            {
                switch (type)
                {
                    case "Incidents":
                        feedURL = "https://ifdsgrouptest.service-now.com/incident_list.do?XML&sysparm_query=number%3E" + lastNumber + "&sysparm_order_by=number&XML&sysparm_record_count=10000";
                        break;
                    case "Problems":
                        feedURL = "https://ifdsgrouptest.service-now.com/problem_list.do?XML&sysparm_query=number%3E" + lastNumber + "&sysparm_order_by=number&XML&sysparm_record_count=10000";
                        break;
                    case "Requests":
                        feedURL = "https://ifdsgrouptest.service-now.com/sc_request_list.do?XML&sysparm_query=number%3E" + lastNumber + "&sysparm_order_by=number&XML&sysparm_record_count=10000";
                        break;
                    case "ChangeRequests":
                        feedURL = "https://ifdsgrouptest.service-now.com/change_request_list.do?XML&sysparm_query=number%3E" + lastNumber + "&sysparm_order_by=number&XML&sysparm_record_count=10000";
                        break;
                    default:
                        break;
                }
                read = DownloadFile(feedURL, tempfilePath);
                lastNumber = XmlToDataTable(tempfilePath, ref xmlDataTable, type);
                if (System.IO.File.Exists(tempfilePath))
                    System.IO.File.Delete(tempfilePath);
            }
            return xmlDataTable;
        }

        public static int DownloadFile(String url, String localFilename)
        {
            // Function will return the number of bytes processed  
            // to the caller. Initialize to 0 here.  
            int bytesProcessed = 0;
            // Assign values to these objects here so that they can  
            // be referenced in the finally block  
            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;
            // Use a try/catch/finally block as both the WebRequest and Stream  
            // classes throw exceptions upon error  
            try
            {
                // Create a request for the specified remote file name  
                WebRequest request = WebRequest.Create(url);
                // Create the credentials required for Basic Authentication  
                System.Net.ICredentials cred = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserName"],ConfigurationManager.AppSettings["PWD"]);
                // Add the credentials to the request  
                request.Credentials = cred;
                if (request != null)
                {
                    // Send the request to the server and retrieve the  
                    // WebResponse object  
                    response = request.GetResponse();
                    if (response != null)
                    {
                        // Once the WebResponse object has been retrieved,  
                        // get the stream object associated with the response's data  
                        remoteStream = response.GetResponseStream();
                        // Create the local file  
                        localStream = File.Create(localFilename);
                        // Allocate a 1k buffer  
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        // Simple do/while loop to read from stream until  
                        // no bytes are returned  
                        do
                        {
                            // Read data (up to 1k) from the stream  
                            bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
                            // Write the data to the local file  
                            localStream.Write(buffer, 0, bytesRead);
                            // Increment total bytes processed  
                            bytesProcessed += bytesRead;
                        } while (bytesRead > 0);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Close the response and streams objects here  
                // to make sure they're closed even if an exception  
                // is thrown at some point  
                if (response != null) response.Close();
                if (remoteStream != null) remoteStream.Close();
                if (localStream != null) localStream.Close();
            }
            // Return total bytes processed to caller.  
            return bytesProcessed;
        }

        public static string XmlToDataTable(string file, ref DataTable xmlDataTable, string type)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(file);
                if (ds.Tables.Count == 0)
                {
                    return "";
                }
                else
                {
                    DataTable dtList = ds.Tables[0];
                    //    //populate the DataTable  
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        //create new row in datatable  
                        DataRow dr = xmlDataTable.NewRow();
                        switch (type)
                        {
                            case "Incidents":
                                dr[Constants.ACTIVE] = Convert.ToBoolean(dtList.Rows[i][0]);
                                dr[Constants.SYSID] = dtList.Rows[i][62];
                                dr[Constants.NUMBER] = dtList.Rows[i][38];
                                dr[Constants.TITLE] = dtList.Rows[i][77];
                                dr[Constants.CATEGORY] = dtList.Rows[i][12];
                                dr[Constants.DESCRIPTION] = dtList.Rows[i][26];
                                dr[Constants.RESOULTION] = dtList.Rows[i][16];
                                switch (Convert.ToString(dtList.Rows[i][56]))
                                {
                                    case "1":
                                        dr[Constants.STATE] = Constants.NEW;
                                        break;
                                    case "2":
                                        dr[Constants.STATE] = Constants.WORK_IN_PROGRESS;
                                        break;
                                    case "6":
                                        dr[Constants.STATE] = Constants.RESOLVED;
                                        break;
                                    case "7":
                                        dr[Constants.STATE] = Constants.CLOSED;
                                        break;
                                    case "10":
                                        dr[Constants.STATE] = Constants.AWAITING_PROBLEM_CLOSURE;
                                        break;
                                    case "11":
                                        dr[Constants.STATE] = Constants.AWAITING_INFORMATION;
                                        break;
                                    case "12":
                                        dr[Constants.STATE] = Constants.SOLUTION_PROPOSED;
                                        break;
                                    case "13":
                                        dr[Constants.STATE] = Constants.REOPENED;
                                        break;
                                    case "14":
                                        dr[Constants.STATE] = Constants.AWAITING_CLIENT_INPUT;
                                        break;
                                    case "16":
                                        dr[Constants.STATE] = Constants.ASSIGNED;
                                        break;
                                    default:
                                        break;
                                }
                                dr[Constants.CLOSE_CODE] = dtList.Rows[i][15];
                                if (Convert.ToString(dtList.Rows[i][39]) != "")
                                    dr[Constants.CREATED_DATE] = Convert.ToDateTime(dtList.Rows[i][39]);
                                if (Convert.ToString(dtList.Rows[i][65]) != "")
                                    dr[Constants.MODIFIED_DATE] = Convert.ToDateTime(dtList.Rows[i][65]);
                                dr[Constants.EXTERNAL_URL] = "https://ifdsgrouptest.service-now.com/incident.do?sys_id=" + Convert.ToString(dtList.Rows[i][62]);
                                break;
                            case "Problems":
                                dr[Constants.ACTIVE] = Convert.ToBoolean(dtList.Rows[i][0]);
                                dr[Constants.SYSID] = dtList.Rows[i][49];
                                dr[Constants.NUMBER] = dtList.Rows[i][30];
                                //dr[Constants.TITLE] = dtList.Rows[i][77];
                                //dr[Constants.CATEGORY] = dtList.Rows[i][12];
                                dr[Constants.DESCRIPTION] = dtList.Rows[i][19];
                                dr[Constants.RESOULTION] = dtList.Rows[i][9];
                                switch (Convert.ToString(dtList.Rows[i][36]))
                                {
                                    case "1":
                                        dr[Constants.STATE] = Constants.OPEN;
                                        break;
                                    case "2":
                                        dr[Constants.STATE] = Constants.WORK_IN_PROGRESS;
                                        break;
                                    case "4":
                                        dr[Constants.STATE] = Constants.CLOSED;
                                        break;
                                    case "5":
                                        dr[Constants.STATE] = Constants.RESOLVED;
                                        break;
                                    case "6":
                                        dr[Constants.STATE] = Constants.AWAITING_CHANGE_IMPLEMENTATION;
                                        break;
                                    case "9":
                                        dr[Constants.STATE] = Constants.AWAITING_INFORMATION;
                                        break;
                                    case "12":
                                        dr[Constants.STATE] = Constants.SOLUTION_PROPOSED;
                                        break;
                                    case "13":
                                        dr[Constants.STATE] = Constants.REOPENED;
                                        break;

                                    default:
                                        break;
                                }
                                dr[Constants.CLOSE_CODE] = dtList.Rows[i][61];
                                if (Convert.ToString(dtList.Rows[i][47]) != "")
                                    dr[Constants.CREATED_DATE] = Convert.ToDateTime(dtList.Rows[i][47]);
                                if (Convert.ToString(dtList.Rows[i][52]) != "")
                                    dr[Constants.MODIFIED_DATE] = Convert.ToDateTime(dtList.Rows[i][52]);
                                dr[Constants.EXTERNAL_URL] = "https://ifdsgrouptest.service-now.com/problem.do?sys_id=" + Convert.ToString(dtList.Rows[i][49]);
                                break;
                            case "Requests":
                                dr[Constants.ACTIVE] = Convert.ToBoolean(dtList.Rows[i][0]);
                                dr[Constants.SYSID] = dtList.Rows[i][53];
                                dr[Constants.NUMBER] = dtList.Rows[i][31];
                                // dr[Constants.TITLE] = dtList.Rows[i][77];
                                //dr[Constants.CATEGORY] = dtList.Rows[i][12];
                                dr[Constants.DESCRIPTION] = dtList.Rows[i][21];
                                dr[Constants.RESOULTION] = dtList.Rows[i][10];
                                dr[Constants.STATE] = Convert.ToString(dtList.Rows[i][40]);

                                //dr[Constants.CLOSE_CODE] = dtList.Rows[i][15];
                                if (Convert.ToString(dtList.Rows[i][51]) != "")
                                    dr[Constants.CREATED_DATE] = Convert.ToDateTime(dtList.Rows[i][51]);
                                if (Convert.ToString(dtList.Rows[i][56]) != "")
                                    dr[Constants.MODIFIED_DATE] = Convert.ToDateTime(dtList.Rows[i][56]);
                                dr[Constants.EXTERNAL_URL] = "https://ifdsgrouptest.service-now.com/sc_request.do?sys_id=" + Convert.ToString(dtList.Rows[i][53]);
                                break;
                            case "ChangeRequests":
                                dr[Constants.ACTIVE] = Convert.ToBoolean(dtList.Rows[i][0]);
                                dr[Constants.SYSID] = dtList.Rows[i][71];
                                dr[Constants.NUMBER] = dtList.Rows[i][39];
                                // dr[Constants.TITLE] = dtList.Rows[i][77];
                                //dr[Constants.CATEGORY] = dtList.Rows[i][12];
                                dr[Constants.DESCRIPTION] = dtList.Rows[i][26];
                                dr[Constants.RESOULTION] = dtList.Rows[i][15];
                                switch (Convert.ToString(dtList.Rows[i][66]))
                                {
                                    case "0":
                                        dr[Constants.STATE] = Constants.DRAFT;
                                        break;
                                    case "1":
                                        dr[Constants.STATE] = Constants.OPEN;
                                        break;
                                    case "3":
                                        dr[Constants.STATE] = Constants.CLOSED_COMPLETE;
                                        break;
                                    case "4":
                                        dr[Constants.STATE] = Constants.CLOSED_INCOMPLETE;
                                        break;
                                    case "5":
                                        dr[Constants.STATE] = Constants.CANCELLED;
                                        break;
                                    case "15":
                                        dr[Constants.STATE] = Constants.CLOSED;
                                        break;
                                    case "18":
                                        dr[Constants.STATE] = Constants.COMPLETE;
                                        break;
                                    case "19":
                                        dr[Constants.STATE] = Constants.PLANNING;
                                        break;
                                    case "20":
                                        dr[Constants.STATE] = Constants.REQUEST_FOR_APPROVAL;
                                        break;
                                    case "21":
                                        dr[Constants.STATE] = Constants.SCHEDULED_FOR_REVIEW;
                                        break;
                                    case "22":
                                        dr[Constants.STATE] = Constants.PENDING_AUTHORIZATION;
                                        break;
                                    case "23":
                                        dr[Constants.STATE] = Constants.SCHEDULED;
                                        break;
                                    case "24":
                                        dr[Constants.STATE] = Constants.IMPLEMENTING;
                                        break;

                                    default:
                                        break;
                                }

                                //dr[Constants.CLOSE_CODE] = dtList.Rows[i][15];
                                if (Convert.ToString(dtList.Rows[i][69]) != "")
                                    dr[Constants.CREATED_DATE] = Convert.ToDateTime(dtList.Rows[i][69]);
                                if (Convert.ToString(dtList.Rows[i][74]) != "")
                                    dr[Constants.MODIFIED_DATE] = Convert.ToDateTime(dtList.Rows[i][74]);
                                dr[Constants.EXTERNAL_URL] = "https://ifdsgrouptest.service-now.com/change_request.do?sys_id=" + Convert.ToString(dtList.Rows[i][71]);
                                break;
                            default:
                                break;
                        }
                        dr[Constants.TYPE] = type;
                        xmlDataTable.Rows.Add(dr);
                    }
                    return xmlDataTable.Rows[xmlDataTable.Rows.Count - 1][Constants.NUMBER].ToString();
                }
            }
            catch (Exception ex)
            {
                return "";
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
                SqlBulkCopy bulkCopy = new SqlBulkCopy(SqlConnectionObj, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.WriteToServer(dt);
                isSuccuss = true;
            }
            catch (Exception ex)
            {
                isSuccuss = false;
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
            string connectionString = ConfigurationManager.AppSettings["connectionString"];
            SqlConnection con = new SqlConnection(connectionString);
            return con;
        }
    }
}
