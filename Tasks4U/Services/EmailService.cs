using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tasks4U.Contracts;
using Tasks4U.Models;

namespace Tasks4U.Services
{
    public class EmailService : IEmailService
    {
        private readonly Uri _emailServiceUri;
        private readonly IHttpClientFactory _clientFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, IHttpClientFactory clientFactory, UserManager<ApplicationUser> userManager, ILogger<EmailService> logger)
        {
             _emailServiceUri = config.GetValue<Uri>("EmailServiceUri");
            _clientFactory = clientFactory;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toUserName, string fromUserName, string subject, string body)
        {
            ApplicationUser toUser = 
                await _userManager.Users.FirstOrDefaultAsync(u => u.DisplayName == toUserName);
            ApplicationUser FromUser = 
                await _userManager.Users.FirstOrDefaultAsync(u => u.DisplayName == fromUserName);

            if (string.IsNullOrEmpty(toUser.Email))
            {
                _logger.LogWarning("Email can't be sent to the receiver of the task. Receiver Email is empty.");
            }
            try
            {
                await SendEmailAsyncInternal(new EmailInfo
                {
                    To = toUser.Email,
                    Cc = FromUser?.Email ?? "",
                    Subject = subject,
                    Body = body
                });
                _logger.LogInformation("Email to task receiver is sent");

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Sending email failed");
            }

        }

        private async Task SendEmailAsyncInternal(EmailInfo emailInfo)
        {
            HttpClient client = _clientFactory.CreateClient("emailClient");
            string message = JsonSerializer.Serialize(emailInfo);
            using HttpContent httpContent = new StringContent(message, Encoding.UTF8);
            HttpResponseMessage response = await client.PostAsync(_emailServiceUri, httpContent);
            response.EnsureSuccessStatusCode();
        }
    }
}
