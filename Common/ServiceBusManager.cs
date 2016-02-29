using System.Configuration;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Common
{
    public class ServiceBusManager
    {
        private readonly string _serviceBusNamespaceConnectionString;
        private string QueueName = "resizepicturesqueue";

        public ServiceBusManager(string serviceBusNamespaceConnectionString)
        {
            _serviceBusNamespaceConnectionString = serviceBusNamespaceConnectionString;
        }


        public void CreateQueue()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_serviceBusNamespaceConnectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }
        }

        public async Task Enqueue(object message)
        {
            var brokeredMessage = new BrokeredMessage(message);

            var topicClient = QueueClient.CreateFromConnectionString(_serviceBusNamespaceConnectionString, QueueName);
            await topicClient.SendAsync(brokeredMessage);
        }

        public static ServiceBusManager CreateDefault()
        {
            var serviceBusConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsServiceBus"];
            if (serviceBusConnectionString != null)
            {
                return new ServiceBusManager(serviceBusConnectionString.ConnectionString);
            }

            return null;
        }
    }
}