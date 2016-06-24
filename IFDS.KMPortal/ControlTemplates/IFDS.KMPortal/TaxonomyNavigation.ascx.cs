using IFDS.KMPortal.Helper;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IFDS.KMPortal.ControlTemplates.IFDS.KMPortal
{
    public class DynamicContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        private IList<string> _propertiesToSerialize = null;

        public DynamicContractResolver(IList<string> propertiesToSerialize)
        {
            _propertiesToSerialize = propertiesToSerialize;
        }

        private bool IsEmptyCollection(JsonProperty property, object target)
        {
            var value = property.ValueProvider.GetValue(target);
            var collection = value as ICollection;
            if (collection != null && collection.Count == 0)
                return true;

            if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                return false;

            var countProp = property.PropertyType.GetProperty(Constants.Count);
            if (countProp == null)
                return false;

            var count = (int)countProp.GetValue(value, null);
            return count == 0;
        }

        protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            Predicate<object> shouldSerialize = property.ShouldSerialize;
            property.ShouldSerialize = obj => (shouldSerialize == null || shouldSerialize(obj)) && !IsEmptyCollection(property, obj);


            if (property.DeclaringType == typeof(TreeNode))
            {
                if (property.PropertyName.Equals("ChildNodes", StringComparison.OrdinalIgnoreCase))
                {
                    property.PropertyName = "nodes";
                }
                if (property.PropertyName.Equals("Nodes", StringComparison.OrdinalIgnoreCase))
                {
                    property.PropertyName = "nodes";
                }
                if (property.PropertyName.Equals("Text", StringComparison.OrdinalIgnoreCase))
                {
                    property.PropertyName = "text";
                }
            }
            return property;
        }

        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            IList<Newtonsoft.Json.Serialization.JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            return properties.Where(p => _propertiesToSerialize.Contains(p.PropertyName)).ToList();
        }
    }


    public partial class TaxonomyNavigation : UserControl
    {
        static TreeNode treeNode = new TreeNode();

        public string WebpartTitle { get; set; }

        public string Termname { get; set; }

        public string HTMLTreeDivId { get; set; }

        public double CachingTimeOut { get; set; }

        public string JSONOutputJSVariable { get; set; }

        public string JSONFinaloutput { get; set; }

        private string _cacheid { get; set; }

        private string _termguid { get; set; }

        private static void AddTermSet(Term term, TreeNode treeNode)
        {
            if (!term.Name.ToLower().EndsWith(Constants.DepartmentTaxonomyText) && !term.Name.ToLower().EndsWith(Constants.ProductTaxonomyText))
            {
                TreeNode termnode = new TreeNode(term.Name);
                termnode.Value = term.Id.ToString();
                treeNode.ChildNodes.Add(termnode);
                treeNode = termnode;
            }
            foreach (Term t in term.Terms)
            {

                AddTermSet(t, treeNode);
            }
        }

        private string GetValuefromPropertyBag(string propertyname)
        {
            string value = string.Empty;
            try
            {
                SPWeb web = SPContext.Current.Web;

                if (web.AllProperties.ContainsKey(propertyname))
                {
                    value = web.AllProperties[propertyname].ToString();
                }
                if (string.IsNullOrEmpty(value))
                {
                    ErrorMessage.Text = "Could not able to get value from Property bag -> Check Property bag Key & values";
                    Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation GetValuefromPropertyBag", "Could not able to get value from Property bag -> Check Property bag Key & values");


                }
            }
            catch (Exception e)
            {
                ErrorMessage.Text = "Could not able to get value from Property bag -> Check Property bag Key & values";
                Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation GetValuefromPropertyBag", e.StackTrace.ToString());

            }
            return value;

        }

        private static string JsonSerializeWithoutQuoteInNames(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new JsonTextWriter(stringWriter) { QuoteName = false })
                    new JsonSerializer().Serialize(writer, obj);
                return stringWriter.ToString();
            }
        }


        private TreeNode GenerateTaxonomyTree(string termname)
        {
            if (string.IsNullOrEmpty(termname))
            {
                ErrorMessage.Text = "Guid incorrect or could not find property in Property bag, Please check Property bag";
                Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation GenerateTaxonomyTree", "Term name / Guid not correct");
                return null;
            }

            TreeNode tn = new TreeNode();
            try
            {
                SPSite thisSite = SPContext.Current.Site;

                TaxonomySession session = new TaxonomySession(thisSite);
                TermStore ts = session.DefaultSiteCollectionTermStore;

                Group group = ts.GetGroup(new Guid(termname));
                if (group != null)
                {

                    TreeNode tgnode = new TreeNode(group.Name);

                    tgnode.Value = group.Id.ToString();

                    tn.ChildNodes.Add(tgnode);
                    tn = tgnode;

                    foreach (TermSet termSet in group.TermSets)
                    {
                        TreeNode termsetnode = new TreeNode(termSet.Name);
                        termsetnode.Value = termSet.Id.ToString();
                        tn.ChildNodes.Add(termsetnode);
                        tn = termsetnode;

                        foreach (Term term in termSet.Terms)
                        {
                            AddTermSet(term, tn);
                        }
                        tn = tgnode;                       
                    }
                }
                else
                {
                    TermSet termSet = ts.GetTermSet(new Guid(termname));

                    if (termSet != null)
                    {
                        TreeNode termsetnode = new TreeNode(termSet.Name);
                        termsetnode.Value = termSet.Id.ToString();
                        tn.ChildNodes.Add(termsetnode);
                        tn = termsetnode;

                        foreach (Term term in termSet.Terms)
                        {
                            AddTermSet(term, tn);
                        }
                    }
                    else
                    {
                        Term term = ts.GetTerm(new Guid(termname));
                        if (term != null)
                        {
                            TreeNode termnode = new TreeNode(term.Name);
                            termnode.Value = term.Id.ToString();
                            tn.ChildNodes.Add(termnode);
                            tn = termnode;

                            foreach (Term t in term.Terms)
                            {
                                AddTermSet(t, tn);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessage.Text = "Error generating Tree structure for the Taxonom";
                Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation GenerateTaxonomyTree", "Error generating Tree structure for the Taxonomy" + e.ToString());
            }
            return tn;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.WebpartTitle))
            {
                ErrorMessage.Text = "Webpart Title property for Usercontrol not specified in Master page.\n\r";
                Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation Page_Load", "Webpart Title property for Usercontrol not specified in Master page.");
                return;
            }
            if (string.IsNullOrEmpty(this.Termname))
            {
                ErrorMessage.Text = "Termname property for Usercontrol not specified in Master page.\n\r";
                Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation Page_Load", "Termname property for Usercontrol not specified in Master page.");
                return;
            }
            if (string.IsNullOrEmpty(this.HTMLTreeDivId))
            {
                ErrorMessage.Text = "HTMLTreeDivId property for Usercontrol not specified in Master page.\n\r";
                Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation Page_Load", "HTMLTreeDivId property for Usercontrol not specified in Master page.");
                return;
            }
            if (string.IsNullOrEmpty(this.JSONOutputJSVariable))
            {
                ErrorMessage.Text = "JSONOutputJSVariable property for Usercontrol not specified in Master page.\n\r";
                Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation Page_Load", "JSONOutputJSVariable property for Usercontrol not specified in Master page.");
                return;
            }

            _termguid = GetValuefromPropertyBag(Termname);
            if (_termguid == null)
            {
                ErrorMessage.Text = "JSONOutputJSVariable property for Usercontrol not specified in Master page.";
                Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation Page_Load", "JSONOutputJSVariable property for Usercontrol not specified in Master page.");

            }

            _cacheid = "ID" + _termguid;
            Object Cachereturn = (object)CacheHelper.GetFromCache(_cacheid);

            if (Cachereturn != null)
            {
                JSONFinaloutput = Cachereturn.ToString();
            }
            else
            {
                try
                {
                    TreeNode treeNode = new TreeNode();
                    treeNode = GenerateTaxonomyTree(_termguid);
                    if (treeNode == null)
                    {
                        Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation Page_Load", "Error generating treenode");
                        return;
                    }
                    List<string> propertiesToSerialize = new List<string>(new string[]
                    {
                    "text",
                    "nodes",
                    "Value"
                    });
                    DynamicContractResolver contractResolver = new DynamicContractResolver(propertiesToSerialize);
                    var results = JsonConvert.SerializeObject(treeNode.ChildNodes, Newtonsoft.Json.Formatting.Indented,
                                    new JsonSerializerSettings()
                                    {
                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                        NullValueHandling = NullValueHandling.Ignore,
                                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                                        ContractResolver = contractResolver

                                    });

                    var jsondata = JsonSerializeWithoutQuoteInNames(JsonConvert.DeserializeObject(results)).ToString();
                    object o = (object)jsondata;
                    if (jsondata != null)
                    {
                        CacheHelper.SaveTocache(_cacheid, o, DateTime.Now.AddMinutes(CachingTimeOut));
                        JSONFinaloutput = jsondata;
                    }
                }
                catch
                {
                    ErrorMessage.Text = "Error converting Taxonomy tree to JSON";
                    Logger.WriteLog(Logger.Category.Unexpected, "KMPortalException-TaxonomyNavigation Page_Load", "Error converting Taxonomy tree to JSON");
                }
            }
        }
    }
}
