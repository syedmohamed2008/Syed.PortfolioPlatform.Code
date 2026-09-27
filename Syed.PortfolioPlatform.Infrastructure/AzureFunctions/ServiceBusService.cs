using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Syed.PortfolioPlatform.Application.Interfaces;

namespace Syed.PortfolioPlatform.Infrastructure.AzureFunctions
{
    public class ServiceBusService : IMessageBusService
    {
        private readonly IConfiguration _configuration;

        public ServiceBusService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishArticleAsync(string message)
        {
            var connectionString = _configuration["ServiceBus:ConnectionString"];
            var queueName = _configuration["ServiceBus:QueueName"];

            await using var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            var serviceBusMessage = new ServiceBusMessage(message)
            {
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
