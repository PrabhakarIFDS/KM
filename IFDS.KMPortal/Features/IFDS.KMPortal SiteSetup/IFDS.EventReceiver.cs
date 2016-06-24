using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using IFDS.KMPortal.Helper;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.Search.Administration.Query;

namespace IFDS.KMPortal.Features.IFDS.KMPortal_SiteSetup
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("8f03e1e6-54e5-42ab-ae42-4be2f738c67a")]
    public class IFDSEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

            try
            {
                // Creating Search Centre Subsite

                using (SPSite oSite = properties.Feature.Parent as SPSite)
                {
                    if (oSite != null)
                    {
                        oSite.AllowUnsafeUpdates = true;
                        oSite.RootWeb.AllowUnsafeUpdates = true;
                        
                        // Creating Result Source
                        CreateResultSource(oSite);

                        //Create Groups
                        CreateGroup(oSite);

                        // Create Search SubSite
                      //  oSite.RootWeb.Webs.Add("Search", "Search", "Search", (uint)1033, "SRCHCEN#0", false, false);

                        // Todo: Creating web properties for Left Navigation

                        oSite.AllowUnsafeUpdates = false;
                        oSite.RootWeb.AllowUnsafeUpdates = false;
                    }
                }
            }
           
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "Site Setup event receiver", "Feature Activated"), ex.StackTrace);
            }
           
                
        }
         


  public void CreateResultSource(SPSite oSite)
  
  {
    
                     SPServiceContext context = SPServiceContext.GetContext(oSite);
  
                     SearchServiceApplicationProxy searchProxy = context.GetDefaultProxy(typeof(SearchServiceApplicationProxy)) as SearchServiceApplicationProxy;
  
                     FederationManager fedManager = new FederationManager(searchProxy);
  
                     SearchObjectOwner oSearchObjectOwner = new SearchObjectOwner(SearchObjectLevel.SPSite, oSite.RootWeb);
  
                     Source oNewResultSource = fedManager.CreateSource(oSearchObjectOwner);
  
                     oNewResultSource.Name = oSite.RootWeb.Title + " Result Source" ;
  
                     oNewResultSource.Description = "Department Site Result Source";
  
                     oNewResultSource.ProviderId = fedManager.ListProviders()["Local SharePoint Provider"].Id;
  
                     Microsoft.Office.Server.Search.Query.Rules.QueryTransformProperties QueryProperties = new Microsoft.Office.Server.Search.Query.Rules.QueryTransformProperties();

                    // String resultSourceQuery = "{searchTerms} path:"+oSite.Url;
                     String resultSourceQuery = "{searchTerms} Path:{SiteCollection.URL}";
      
  
                     oNewResultSource.CreateQueryTransform(QueryProperties,resultSourceQuery );
  
                     oNewResultSource.Commit();
  
 }

  private void CreateGroup(SPSite oSite)
  {
      SPSite site = oSite;
      SPWeb web = null;
      if (site != null)
      {
          web = site.OpenWeb();
      }
      
      SPGroup ALL_IFDS_User = null;
      SPGroup IFDS_department_contribute = null;
      SPGroup IFDS_KMPortal_global_admin = null;
      SPGroup IFDS_department_admin = null;
      SPGroup IFDS_department_approver = null;
      SPGroup IFDS_KMPortal_analyst = null;
      SPGroup IFDS_department_KMPortal_champion = null;

      //creating groups
      try
      {
          if (web != null)
          {
              // ALL_IFDS_User Group

              bool isALLIFDSUserExists = GroupExists(web, "ALL_IFDS_User");

              if (!isALLIFDSUserExists)
              {
                  SPRoleDefinition roleDefinition = web.RoleDefinitions.GetByType(SPRoleType.Reader);
                  ALL_IFDS_User = AddGroup(web, "ALL_IFDS_User", "All IFDS Users's Group", roleDefinition);
              }
              else
              {
                  ALL_IFDS_User = web.SiteGroups["ALL_IFDS_User"];
              }

              // IFDS_department_contribute Group

              string ifdsDeptContr = string.Format("{0}_{1}_{2}", "IFDS", web.Title,"Contribute");
              bool isIFDSDeptContributeGroupExists = GroupExists(web, ifdsDeptContr);

              if (!isIFDSDeptContributeGroupExists)
              {
                  SPRoleDefinition roleDefinition = web.RoleDefinitions.GetByType(SPRoleType.Contributor);
                  IFDS_department_contribute = AddGroup(web, ifdsDeptContr, string.Format("{0} {1}", ifdsDeptContr, "Group"), roleDefinition);
              }
              else
              {
                  IFDS_department_contribute = web.SiteGroups[ifdsDeptContr];
              }

              // IFDS_KM Portal_global admin Group

              bool isIFDSGlobalAdminGroupExists = GroupExists(web, "IFDS_KMPortal_Admin_Group");

              if (!isIFDSGlobalAdminGroupExists)
              {
                  SPRoleDefinition roleDefinition = web.RoleDefinitions.GetByType(SPRoleType.Administrator);
                  IFDS_KMPortal_global_admin = AddGroup(web, "IFDS_KMPortal_Admin_Group", "IFDS KMPOrtal Administrator Group", roleDefinition);
              }
              else
              {
                  IFDS_KMPortal_global_admin = web.SiteGroups["IFDS_KMPortal_Admin_Group"];
              }

              // IFDS_department_admin Group

              string ifdsDeptAdmin = string.Format("{0}_{1}_{2}", "IFDS", web.Title,"Admin");
              bool isIFDSDeptAdminGroupExists = GroupExists(web, ifdsDeptAdmin);

              if (!isIFDSDeptAdminGroupExists)
              {
                  SPRoleDefinition roleDefinition = web.RoleDefinitions.GetByType(SPRoleType.Administrator);
                  IFDS_department_admin = AddGroup(web, ifdsDeptAdmin, string.Format("{0} {1}", ifdsDeptAdmin, "Group"), roleDefinition);
              }
              else
              {
                  IFDS_department_admin = web.SiteGroups[ifdsDeptAdmin];
              }

              // IFDS_department_approver Group

              string ifdsDeptAppr = string.Format("{0}_{1}_{2}", "IFDS", web.Title, "Approver");
              bool isIFDSDeptApproverGroupExists = GroupExists(web, ifdsDeptAppr);

              if (!isIFDSDeptApproverGroupExists)
              {
                  SPRoleDefinition roleDefinition = web.RoleDefinitions.GetByType(SPRoleType.Contributor);
                  IFDS_department_approver = AddGroup(web, ifdsDeptAppr, string.Format("{0} {1}", ifdsDeptAppr, "Group"), roleDefinition);
              }
              else
              {
                  IFDS_department_approver = web.SiteGroups[ifdsDeptAppr];
              }

              // IFDS_KM Portal_analyst Group

              bool isIFDSKMAnalystGroupExists = GroupExists(web, "IFDS_KMPortal_Analyst");

              if (!isIFDSKMAnalystGroupExists)
              {
                  SPRoleDefinition roleDefinition = web.RoleDefinitions.GetByType(SPRoleType.Contributor);
                  IFDS_KMPortal_analyst = AddGroup(web, "IFDS_KMPortal_Analyst", "IFDS KMPortal Analyst Group", roleDefinition);
              }
              else
              {
                  IFDS_KMPortal_analyst = web.SiteGroups["IFDS_KMPortal_Analyst"];
              }


              // IFDS_department_KMPortal_Champion Group

              string ifdsDeptChamp = string.Format("{0}_{1}_{2}", "IFDS", web.Title, "KMPortal_Champion");
              bool isIFDSDeptChampGroupExists = GroupExists(web, ifdsDeptChamp);

              if (!isIFDSDeptChampGroupExists)
              {
                  SPRoleDefinition roleDefinition = web.RoleDefinitions.GetByType(SPRoleType.Contributor);
                  IFDS_department_KMPortal_champion = AddGroup(web, ifdsDeptChamp, string.Format("{0} {1}", ifdsDeptChamp, "Group"), roleDefinition);
              }
              else
              {
                  IFDS_department_KMPortal_champion = web.SiteGroups[ifdsDeptChamp];
              }

              
          }
      }
      catch (Exception ex)
      {
          Logger.WriteLog(Logger.Category.Unexpected, string.Format("{0}-{1}", "Site Setup event receiver- Create Group", "Feature Activated"), ex.StackTrace);
      }
  }

  private SPGroup AddGroup(SPWeb web, string GroupName, string desc, SPRoleDefinition role)
  {
      web.SiteGroups.Add(GroupName, web.CurrentUser, web.CurrentUser, desc);
      SPGroup objgroup = web.SiteGroups[GroupName];

      // Add the group's permissions

      SPRoleAssignment roleAssignment = new SPRoleAssignment(objgroup);
      roleAssignment.RoleDefinitionBindings.Add(role);
      web.RoleAssignments.Add(roleAssignment);
      web.Update();
      return objgroup;
  }

  private bool GroupExists(SPWeb web, string groupName)
  {
      bool isvalid = true;

      SPGroupCollection groupCollection = web.Groups;

      foreach (SPGroup item in groupCollection)
      {
          if (item.Name == groupName)
          {
              isvalid = true;
              break;
          }
          else
          {
              isvalid = false;
          }
      }

      return isvalid;
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
