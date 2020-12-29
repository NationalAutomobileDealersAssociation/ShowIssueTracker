using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowIssueTracker.Repositories
{
    public class SendGridProperties
    {
        public string ApiKey { get; set; }

        public string FromEmailAddress { get; set; }

        public string FromName { get; set; }
    }
}
