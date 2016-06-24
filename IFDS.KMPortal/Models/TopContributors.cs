using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFDS.KMPortal.Models
{
    public class TopContributors
    {
        public string Member { get; set; }
        public string GiftedBadgeText { get; set; }
        public int NoOfReplies { get; set; }
        public int NoOfDiscussions { get; set; }
        public int ReputationScore { get; set; }
        public string emailaddress { get; set; }
        public string phonenumber { get; set; }
        public string personalurl { get; set; }
    }
}
