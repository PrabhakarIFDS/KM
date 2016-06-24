using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace IFDS.KMPortal.BDC_CornerStone
{
    /// <summary>
    /// All the methods for retrieving, updating and deleting data are implemented in this class file.
    /// The samples below show the finder and specific finder method for Entity1.
    /// </summary>
    public class CornerStone_CourseCatalogService
    {
        /// <summary>
        /// This is a sample specific finder method for Entity1.
        /// If you want to delete or rename the method think about changing the xml in the BDC model file as well.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Entity1</returns>
        public static CornerStone_CourseCatalog ReadItem(string id)
        {
            //TODO: This is just a sample. Replace this simple sample with valid code.
            //CornerStone_CourseCatalog CourseCatolog = new CornerStone_CourseCatalog();
            //entity1.Title = id;
            //entity1.Message = "Hello World";
            //return entity1;
            SqlConnection conn = GetSQLConnection();
            CornerStone_CourseCatalog CourseCatolog = new CornerStone_CourseCatalog();
            try
            {
               
                
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT Title,CreatedDate,Description,SubjectIDs,SubjectTitles,Price,ProviderName,Type,DeepLinkURL,CourseDuration,OUAvailability,LOTitles,LOInstructions,LOLocations,LOInstructors FROM CornerStone_CourseCatalog";
                cmd.Connection = conn;
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (rdr.Read())
                {

                    CourseCatolog.Title = rdr[0] == null ? string.Empty : rdr[0].ToString();
                    CourseCatolog.CreatedDate = rdr[1] == null ? DateTime.MinValue : Convert.ToDateTime(rdr[1].ToString());
                    CourseCatolog.Description = rdr[2] == null ? string.Empty : rdr[2].ToString();
                    CourseCatolog.SubjectIDs = rdr[3] == null ? string.Empty : rdr[3].ToString();
                    CourseCatolog.SubjectTitles = rdr[4] == null ? string.Empty : rdr[4].ToString();
                    CourseCatolog.Price = rdr[5] == null ? 0 : Convert.ToDecimal(rdr[5].ToString());
                    CourseCatolog.Type = rdr[6] == null ? string.Empty : rdr[6].ToString();
                    CourseCatolog.DeepLinkURL = rdr[7] == null ? string.Empty : rdr[7].ToString();
                    CourseCatolog.CourseDuration = rdr[8] == null ? string.Empty : rdr[8].ToString();
                    CourseCatolog.OUAvailability = rdr[9] == null ? string.Empty : rdr[9].ToString();
                    CourseCatolog.LOTitles = rdr[10] == null ? string.Empty : rdr[10].ToString();
                    CourseCatolog.LOInstructions = rdr[11] == null ? string.Empty : rdr[11].ToString();
                }
            }
            // conn.Dispose();
            catch (Exception ex)
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Insert into IFDSLogs values ('" + ex.ToString() + "')";
                cmd.ExecuteNonQuery();

                return CourseCatolog;

            }
            finally
            {
                conn.Dispose();
            }
            return CourseCatolog;
        }

         private static SqlConnection GetSQLConnection()
        {
            string connectionString = "Integrated Security=SSPI;Persist Security Info=True;server=CAD2CA1VSPW01;Initial Catalog=ExternalSystems";
            SqlConnection con = new SqlConnection(connectionString);
            return con;
        }
        /// <summary>
        /// This is a sample finder method for Entity1.
        /// If you want to delete or rename the method think about changing the xml in the BDC model file as well.
        /// </summary>
        /// <returns>IEnumerable of Entities</returns>
        public static IEnumerable<CornerStone_CourseCatalog> ReadList()
        { 

            SqlConnection conn = GetSQLConnection();
            CornerStone_CourseCatalog[] CourseCatologs = new CornerStone_CourseCatalog[0];
            try
            {

               List<CornerStone_CourseCatalog> allCourseCatolog = new List<CornerStone_CourseCatalog>();

                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT Title,CreatedDate,Description,SubjectIDs,SubjectTitles,Price,ProviderName,Type,DeepLinkURL,CourseDuration,OUAvailability,LOTitles,LOInstructions,LOLocations,LOInstructors FROM CornerStone_CourseCatalog";
                cmd.Connection = conn;
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (rdr.Read())
                {

                    CornerStone_CourseCatalog CourseCatolog = new CornerStone_CourseCatalog(); 
                    CourseCatolog.Title = rdr[0] == null ? string.Empty : rdr[0].ToString();
                    CourseCatolog.CreatedDate = rdr[1] == null ? DateTime.MinValue : Convert.ToDateTime(rdr[1].ToString());
                    CourseCatolog.Description = rdr[2] == null ? string.Empty : rdr[2].ToString();
                    CourseCatolog.SubjectIDs = rdr[3] == null ? string.Empty : rdr[3].ToString();
                    CourseCatolog.SubjectTitles = rdr[4] == null ? string.Empty : rdr[4].ToString();
                    CourseCatolog.Price = rdr[5] == null ? 0 : Convert.ToDecimal(rdr[5].ToString());
                    CourseCatolog.Type = rdr[6] == null ? string.Empty : rdr[6].ToString();
                    CourseCatolog.DeepLinkURL = rdr[7] == null ? string.Empty : rdr[7].ToString();
                    CourseCatolog.CourseDuration = rdr[8] == null ? string.Empty : rdr[8].ToString();
                    CourseCatolog.OUAvailability = rdr[9] == null ? string.Empty : rdr[9].ToString();
                    CourseCatolog.LOTitles = rdr[10] == null ? string.Empty : rdr[10].ToString();
                    CourseCatolog.LOInstructions = rdr[11] == null ? string.Empty : rdr[11].ToString();
                    allCourseCatolog.Add(CourseCatolog);
                }
                CourseCatologs = new CornerStone_CourseCatalog[allCourseCatolog.Count];
                for (int j = 0; j < allCourseCatolog.Count; j++)
                {
                    CourseCatologs[j] = allCourseCatolog[j];
                }
                return CourseCatologs;
            }
            catch (Exception ex)
            {
                conn.Open();
                DateTime dt = DateTime.Now;
                SqlCommand cmd = new SqlCommand("Insert into DWR_Z_IFDSLogs values ('" + ex.ToString() + "','" + dt + "')", conn);
                cmd.ExecuteNonQuery();
                //CourseCatologs = new CornerStone_CourseCatalog[0];
                //CornerStone_CourseCatalog CourseCatolog = new CornerStone_CourseCatalog();
                //CourseCatolog.Title = "error";
                //CourseCatolog.CreatedDate = DateTime.MinValue;
                //CourseCatolog.Description = "error";
                //CourseCatolog.SubjectIDs = "error";
                //CourseCatolog.SubjectTitles = "error";
                //CourseCatolog.Price = 0;
                //CourseCatolog.Type = "error";
                //CourseCatolog.DeepLinkURL = "error";
                //CourseCatolog.CourseDuration = "error";
                //CourseCatolog.OUAvailability = "error";
                //CourseCatolog.LOTitles = "error";
                //CourseCatolog.LOInstructions = "error";
                //CourseCatologs[0] = CourseCatolog;
                //Customer customer = new Customer();
                //customer.CustomerID = -1;
                //customer.CustomerName = "Not found";
                //customer.City = "";
                //customers[0] = customer;
                return CourseCatologs;

            }
            finally
            {
                conn.Dispose();
            }
            //return allCustomers;
        }
       
    }
}
