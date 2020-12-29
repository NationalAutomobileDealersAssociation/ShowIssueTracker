using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowIssueTracker.Models
{
    public class ImpexiumProperties
    {
        public string AccessEndPoint { get; set; }

        public string AppKey { get; set; }

        public string AppId { get; set; }

        public string ApiAccessPassword { get; set; }

        public string ApiAccessEmail { get; set; }

        public string NadaCopyDb { get; set; }

        public string SmtpClient { get; set; }
    }
}
