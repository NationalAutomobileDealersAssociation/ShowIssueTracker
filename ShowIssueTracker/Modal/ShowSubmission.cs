using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
namespace ShowIssueTracker.Modal
{
    public class ShowSubmission : TableEntity
    {
        public ShowSubmission()
        {
        }

        public ShowSubmission(string firstName, string lastName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
            
        }

        public string Company { get; set; }
        public string Title { get; set; }

        public string Email { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; } 
        public string BlobUrl { get; set; }
        public string description { get; set; }
        /*new*/
        public string YearForm { get; set; }
        public string Form { get; set; }

    }
}
