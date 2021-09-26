using System.Net.Http;
using System.Threading.Tasks;
using Tasks4U.Contracts;

namespace Tasks4U.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toUserName, string fromUserName, string subject, string body);
    }
}