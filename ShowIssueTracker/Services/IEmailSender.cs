using System.Threading.Tasks;

namespace ShowIssueTracker.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
