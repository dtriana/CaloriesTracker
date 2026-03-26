using CaloriesTracker.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CaloriesTracker.Services
{
    // Simple mock email sender that logs emails instead of sending
    public class MockEmailSender : IEmailSender
    {
        private readonly ILogger<MockEmailSender> _logger;
        public MockEmailSender(ILogger<MockEmailSender> logger) => _logger = logger;

        public Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            _logger.LogInformation("[MockEmail] To: {To}, Subject: {Subject}, Body: {Body}", to, subject, htmlMessage);
            return Task.CompletedTask;
        }
    }
}
