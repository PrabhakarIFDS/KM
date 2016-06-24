using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using Microsoft.Office.Server.Search.ContentProcessingEnrichment;
using Microsoft.Office.Server.Search.ContentProcessingEnrichment.PropertyTypes;

namespace KMPortalEnrichmentService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "KMPortalContentEnrichmentService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select KMPortalContentEnrichmentService.svc or KMPortalContentEnrichmentService.svc.cs at the Solution Explorer and start debugging.
    public class KMPortalContentEnrichmentService : IContentProcessingEnrichmentService
    {
        public ProcessedItem ProcessItem(Item item)
        {


           
            ProcessedItem processedItem = new ProcessedItem
            {
                ItemProperties = new List<AbstractProperty>()
            };

            WriteLog("Start to process content enrichment...\n");
           /// processedItem.ItemProperties.Clear();
            try
            {

                // Iterate over each property received and locate the two properties we
                // configured the system to send.
                foreach (var property in item.ItemProperties)
                {
                    if (property.Name.Equals("LOLocations", StringComparison.Ordinal))
                    {
                        //item.

                        var categoryProperty = item.ItemProperties.Where(p => p.Name == "LOLocations").FirstOrDefault();
                        Property<List<string>> categoryProp = categoryProperty as Property<List<string>>;

                        if (categoryProp != null && categoryProp.Value != null && categoryProp.Value.Count > 0)
                        {
                            string[] propValues = categoryProp.Value.First().Split('>');
                            List<string> listpropValues = propValues.ToList();
                            propValues.ToList().Remove("");
                            Property<List<string>> newCategoryProp = new Property<List<string>>();
                            //LOLocationsMulti
                            newCategoryProp.Name = "LOLocationsMulti";
                            newCategoryProp.Value = listpropValues;
                            processedItem.ItemProperties.Add(newCategoryProp);
                            WriteLog(string.Format("{0} values are added to  property {1}. with values {2}", newCategoryProp.Value.Count, newCategoryProp.Name, categoryProp.Value.First()));
                            //TimeSpan
                        }
                    }
                    else if (property.Name.Equals("SubjectTitles", StringComparison.Ordinal))
                    {
                        var categoryProperty = item.ItemProperties.Where(p => p.Name == "SubjectTitles").FirstOrDefault();
                        Property<List<string>> categoryProp = categoryProperty as Property<List<string>>;

                        if (categoryProp != null && categoryProp.Value != null && categoryProp.Value.Count > 0)
                        {
                            string[] propValues = categoryProp.Value.First().Split(',');
                            List<string> listpropValues = propValues.ToList();
                            propValues.ToList().Remove("");
                            Property<List<string>> newCategoryProp = new Property<List<string>>();
                            //LOLocationsMulti
                            newCategoryProp.Name = "SubjectTitlesMulti";
                            newCategoryProp.Value = listpropValues;
                            processedItem.ItemProperties.Add(newCategoryProp);
                            WriteLog(string.Format("{0} values are added to  property {1}. with values {2}", newCategoryProp.Value.Count, newCategoryProp.Name, categoryProp.Value.First()));
                            //TimeSpan
                        }
                    }
                }

                //SubjectsTitleMulti
            }

            catch (Exception ex)
            {
                WriteLog(string.Format("Exception is encountered. Error Message: {0}", ex.Message + "\n\t" + ex.StackTrace));
            }
            return processedItem;
        }

        private void WriteLog(string msg)
        {
            using (var writer = new StreamWriter("C:\\Temp\\ContentEnrichmentWebServiceTraceLog_" + System.DateTime.Today.ToString("yyyyMMdd") + ".txt", true))
            {
                writer.Write(System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "\t" + msg);
                writer.WriteLine("");
            }
        }

    }
}
