using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ShowIssueTracker.Repositories
{

    public class SendEmail
    {
        public string Name { get; set; }
        [Required]
        public string ToEmail { get; set; }

        public string CCEmail { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
