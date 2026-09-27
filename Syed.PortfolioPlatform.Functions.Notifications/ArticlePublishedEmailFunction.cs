using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Syed.PortfolioPlatform.Functions.Notifications.Modules;
using System;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;

namespace Syed.PortfolioPlatform.Functions.Notifications
{
    public class ArticlePublishedEmailFunction
    {
        private readonly ILogger<ArticlePublishedEmailFunction> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;

        public ArticlePublishedEmailFunction(
            ILogger<ArticlePublishedEmailFunction> logger,
            IConfiguration configuration,
             AppDbContext dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [Function(nameof(ArticlePublishedEmailFunction))]
        public async Task Run(
            [ServiceBusTrigger("article-published", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            try
            {
                _logger.LogInformation("=== FUNCTION STARTED ===");
                _logger.LogInformation($"Message received: {message}");

                var body = message.Body.ToString();

                var article = JsonSerializer.Deserialize<ArticleEvent>(body);

                var apiKey = _configuration["SendGridApiKey"];
                var fromEmail = _configuration["FromEmail"];

                var client = new SendGridClient(apiKey);

                var contacts = await _dbContext.contactMessages.ToListAsync();

                foreach (var contact in contacts)
                {
                    var msg = new SendGridMessage()
                    {
                        From = new EmailAddress(fromEmail, "Portfolio"),
                        Subject = $"New Article: {article.Title}",
                        PlainTextContent = $"A new article was published: {article.Title}",
                        HtmlContent = $"<h2>{article.Title}</h2><p>Published on {article.PublishedDate}</p>"
                    };

                    msg.AddTo(new EmailAddress(contact.Email));

                    var response = await client.SendEmailAsync(msg);

                    _logger.LogInformation("Email sent to {email}, Status: {status}",
                        contact.Email, response.StatusCode);
                }



                await messageActions.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                throw;  
            }
        }
    }
}
