using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowIssueTracker.Repositories
{
    public interface INadaRepository
    {
     Task<string> SendGridEmailAsync(string toAddress, string subject, string body, string ccAddress);

    }
}
