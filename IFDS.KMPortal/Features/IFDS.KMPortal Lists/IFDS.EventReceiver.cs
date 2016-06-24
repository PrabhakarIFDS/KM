using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using IFDS.KMPortal.Helper;
using System.Collections.Generic;
using System.Linq;

namespace IFDS.KMPortal.Features.IFDS.KMPortal_Lists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("1ff79725-d821-4640-bc69-609a8b1b553f")]
    public class IFDSEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                // Creating Search Centre Subsite

                using (SPWeb oweb = properties.Feature.Parent as SPWeb)
                {
                    if (!ListExists(oweb, "Best Practices"))
                    {
                        AddDocumentLibs("Best Practices", "Holds the information about Best Practices", oweb);
                        SPContentType ctBestPractices = oweb.AvailableContentTypes["Best Practices"];
                        if (ctBestPractices != null)
                        {
                            SPList oBestPracticeslist = oweb.Lists["Best Practices"];
                            if (oBestPracticeslist != null)
                            {
                                oBestPracticeslist.ContentTypesEnabled = true;

                                oBestPracticeslist.ContentTypes.Add(ctBestPractices);

                            }
                            SetDefaultContentType(oBestPracticeslist, ctBestPractices.Name);

                        }
                    }

                    if (!ListExists(oweb, "Onboarding Documents"))
                    {
                        AddDocumentLibs("Onboarding Documents", "Holds the information about Onboarding Documents", oweb);
                        SPContentType ctOnboardingDocuments = oweb.AvailableContentTypes["Onboarding Documents"];
                        if (ctOnboardingDocuments != null)
                        {
                            SPList oOnboardingDocumentslist = oweb.Lists["Onboarding Documents"];
                            if (oOnboardingDocumentslist != null)
                            {
                                oOnboardingDocumentslist.ContentTypesEnabled = true;

                                oOnboardingDocumentslist.ContentTypes.Add(ctOnboardingDocuments);
                            }

                            SetDefaultContentType(oOnboardingDocumentslist, ctOnboardingDocuments.Name);
                        }
                    }

                    if (!ListExists(oweb, "FAQ"))
                    {
                        AddCustomLists("FAQ", "Holds the information about FAQ", oweb);
                        SPContentType ctFAQ = oweb.AvailableContentTypes["FAQ"];
                        if (ctFAQ != null)
                        {
                            SPList oFAQlist = oweb.Lists["FAQ"];
                            if (oFAQlist != null)
                            {
                                oFAQlist.ContentTypesEnabled = true;
                                oFAQlist.ContentTypes.Add(ctFAQ);
                            }

                            oweb.AllowUnsafeUpdates = true;
                            oFAQlist.Fields["Title"].Title = "Question";
                            oFAQlist.Fields["Title"].Update();
                            oweb.AllowUnsafeUpdates = true;
                            SetDefaultContentType(oFAQlist, ctFAQ.Name);

                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "IFDS KMPortal Lists event receiver", "Feature Activated"), ex.StackTrace);
            }
           
        }

       
        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {

            //SPWeb web = properties.Feature.Parent as SPWeb; // Assuming web scoped feature
            //SPList oFAQList;
            //bool listExists = true;
            ////Check to see if the list exists, this method sucks but 2007 doesn't have web.TryGetList()
            //try
            //{
            //    oFAQList = web.Lists["FAQ"];

            //    if()
            //}
            //catch (FileNotFoundException e)
            //{
            //    listExists = false;
            //}

            //if (!listExists)
            //{
            //    // Create list and record returned guid
            //    Guid customPagesListGuid = web.Lists.Add("CustomPages",
            //        "Library to store web pages used in the site", SPListTemplateType.DocumentLibrary);
            //    //Get list from stored guid
            //    customPagesList = web.Lists[customPagesListGuid];
            //    // Set list properties and add required content types
            //    customPagesList.Title = "CustomPages";
            //    customPagesList.OnQuickLaunch = false; // Set to true to display on the quick launch
            //    customPagesList.ContentTypesEnabled = true;
            //    customPagesList.NoCrawl = true; // Set to false if you want pages indexed by search
            //    customPagesList.EnableFolderCreation = true;
            //    customPagesList.EnableSyndication = false; // Turn off rss
            //    SPContentType webPartPageCT = web.AvailableContentTypes[SPBuiltInContentTypeId.WebPartPage];
            //    SPContentType basicPageCT = web.AvailableContentTypes[SPBuiltInContentTypeId.BasicPage];
            //    customPagesList.ContentTypes.Add(webPartPageCT);
            //    customPagesList.ContentTypes.Add(basicPageCT);
            //    // Remove the default content type added on list creation if it is not needed
            //    DeleteContentType(customPagesList.ContentTypes, "Document");

            //    // Commit changes                   
            //    customPagesList.Update();

            //    //Get library from stored guid
            //    SPDocumentLibrary customPagesLibrary = (SPDocumentLibrary)web.Lists[customPagesListGuid];
            //    customPagesLibrary.Folders.Add("/Lists/CustomPages/yyy", SPFileSystemObjectType.Folder);
            //    string rootFolderUrl = customPagesLibrary.RootFolder.ServerRelativeUrl;
            //    SPListItem newFolder = customPagesLibrary.Folders.Add(rootFolderUrl, SPFileSystemObjectType.Folder, "yyy");
            //    newFolder.Update();
            //}

        }

        private void AddDocumentLibs(string DocLibName,string Desc,SPWeb web)
        {
            Guid customListGuid = web.Lists.Add(DocLibName, Desc, SPListTemplateType.DocumentLibrary);
            web.Update();
            SPList custList = web.Lists[customListGuid];

            // show the list in quick launch
            custList.OnQuickLaunch = false;
            custList.Update();

            //// set both properties to true if you want both major and minor versioning to be turned on.
            //custList.EnableVersioning = true;
            //custList.EnableMinorVersions = true;
            //custList.Update();
        }

        private void AddCustomLists(string ListName, string Desc, SPWeb web)
        {
            Guid customListGuid = web.Lists.Add(ListName, Desc, SPListTemplateType.GenericList);
            web.Update();
            SPList custList = web.Lists[customListGuid];

            // show the list in quick launch
            custList.OnQuickLaunch = false;
            custList.Update();
        }

        private void SetDefaultContentType(SPList list, string contentTypeName)
        {
            IList<SPContentType> cTypes = new List<SPContentType>();
            SPFolder root = list.RootFolder;
            cTypes = root.ContentTypeOrder;
            SPContentType cType = cTypes.SingleOrDefault(hd => hd.Name == contentTypeName);
            int j = cTypes.IndexOf(cType);
            cTypes.RemoveAt(j);
            cTypes.Insert(0, cType);
            root.UniqueContentTypeOrder = cTypes;
            root.Update();
        }

        private bool ListExists(SPWeb web, string listName)
        {
            var lists = web.Lists;
            foreach (SPList list in lists)
            {
                if (list.Title.Equals(listName))
                    return true;
            }
            return false;
        }
  

        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
