using System;

namespace ShowIssueTracker.Authorization
{
    public interface IGlobalExceptionLoggingMiddleWare
    {
        void Invoke(Exception ex, string message);
    }
}