using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFDS.KMPortal.BDC_CornerStone
{
    /// <summary>
    /// This class contains the properties for Entity1. The properties keep the data for Entity1.
    /// If you want to rename the class, don't forget to rename the entity in the model xml as well.
    /// </summary>
    public partial class CornerStone_CourseCatalog
    {
        //TODO: Implement additional properties here. The property Message is just a sample how a property could look like.
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public string SubjectIDs { get; set; }
        public string SubjectTitles { get; set; }
        public decimal Price { get; set; }
        public string ProviderName { get; set; }
        public string Type { get; set; }
        public string DeepLinkURL { get; set; }
        public string CourseDuration { get; set; }
        public string OUAvailability { get; set; }
        public string LOTitles { get; set; }
        public string LOInstructions { get; set; }
        public string LOLocations { get; set; }
        public string LOInstructors { get; set; }
       
        
        
    }
}
