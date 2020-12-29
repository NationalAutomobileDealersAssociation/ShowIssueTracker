using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowIssueTracker.Models
{
    public class UploadViewModel 
    {

        public string firstName { get; set; }
        public string lastName { get; set; }
        public string MiddleName { get; set; }
        public string DealershipAffiliation { get; set; }
        public string Title { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string YearsInAutomitive { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string BlobUrl { get; set; }
    }
}
