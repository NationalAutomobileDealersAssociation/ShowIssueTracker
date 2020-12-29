using System.Threading.Tasks;

namespace ShowIssueTracker.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
