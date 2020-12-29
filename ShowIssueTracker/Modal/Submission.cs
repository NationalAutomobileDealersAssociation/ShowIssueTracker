using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
namespace ShowIssueTracker.Modal
{
    public class Submission : TableEntity
    {
        public Submission()
        {
        }

        public Submission(string firstName, string lastName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
            
        }

        public string MiddleName { get; set; }
        public string  DealershipAffiliation { get; set; }
        public string Title { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string YearsInAutomitive { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string BlobUrl { get; set; }
        /*new*/
        public string Year { get; set; }
        public string Form { get; set; }

    }
}
