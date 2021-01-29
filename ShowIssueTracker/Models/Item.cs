using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace ShowIssueTracker.Models
{

    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "FullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "Issue")]
        public string Issue { get; set; }

        [JsonProperty(PropertyName = "Role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "IssueType")]
        public string IssueType { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "AssignedTo")]
        public string AssignedTo { get; set; }

        [JsonProperty(PropertyName = "IssueNotes")]
        public string IssueNotes { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public bool Completed { get; set; }

        [JsonProperty(PropertyName = "PublicUrl")]
        public string PublicUrl { get; set; }

        [JsonProperty(PropertyName = "EntryTime")]
        public DateTime EntryTime { get; set; }

        [JsonProperty(PropertyName = "LastSavedBy")]
        public string LastSavedBy { get; set; }

        [JsonProperty(PropertyName = "LastSavedTime")]
        public DateTime LastSavedTime { get; set; }

        [JsonProperty(PropertyName = "Priority")]
        public string Priority { get; set; }

        [JsonProperty(PropertyName = "BlobUrl")]
        public string BlobUrl { get; set; }

        [JsonProperty(PropertyName = "valueINeed")]
        public string valueINeed { get; set; }
        

    }

}
