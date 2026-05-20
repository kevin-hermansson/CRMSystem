using CRM.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;


namespace CRM.Functions
{
    public class CustomerNotificationFunction
    {
        private readonly ILogger _logger;

        public CustomerNotificationFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomerNotificationFunction>();
        }

        [Function("CustomerNotificationFunction")]
        public void Run([CosmosDBTrigger(
            databaseName: "CRMDatabase",
            containerName: "Customers",
            Connection = "CosmosDbConnection",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)] 
        IReadOnlyList<Customer> customers)
        {
            foreach (var customer in customers)
            {
                _logger.LogInformation($"Customer changed: {customer.Name}");

                using var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential(
                        "4016c71f0f300b",
                        "efa07ff4c4a2e9"),

                    EnableSsl = true
                };

                client.Send(
                    "test@mailtrap.io",
                    customer.SalesPerson.Email,
                    "New Customer Assigned",
                    $"A new customer, {customer.Name}, has been assigned to you.");
                    
            }
        }
    }

}
