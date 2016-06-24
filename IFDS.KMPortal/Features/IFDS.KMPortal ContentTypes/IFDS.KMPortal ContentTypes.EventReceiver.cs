using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace IFDS.KMPortal
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("4d709a77-ae16-4f7b-903b-77a35fc5f773")]
    public class DepartmentSiteConfigurationFeatureEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

           // SPSite site = properties.Feature.Parent as SPSite;
           // Guid fieldId = new Guid("{85A452FD-37E7-47B0-A4F1-910EDC0E0650}");
           //// SPField DepartmentTaxonomyField = new SPField() 
           // site.RootWeb.Fields.CreateNewField("", "DepartmentTags");

           // if (site.RootWeb.Fields.Contains(fieldId))
           // {
           //     TaxonomySession session = new TaxonomySession(site);
           //     if (session.TermStores.Count != 0)
           //     {
           //         var termStore = session.TermStores["ManagedMetadata_Proxy"];
           //         var group = termStore.Groups["GroupName"];
           //         var termSet = group.TermSets["TermSetName"];
           //         TaxonomyField field = site.RootWeb.Fields[fieldId] as TaxonomyField;

           //         //set the text field to the id of the OrderStatus_Note field : f47685b7-93eb-45da-8227-c59925a0d87e
           //         field.TextField = new Guid("{f47685b7-93eb-45da-8227-c59925a0d87e}");

           //         // Connect to MMS
           //         field.SspId = termSet.TermStore.Id;
           //         field.TermSetId = termSet.Id;
           //         field.TargetTemplate = string.Empty;
           //         field.AnchorId = Guid.Empty;
           //         field.Update();
           //     }
           // }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


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
