using CRM.Api.Models;
using Microsoft.Azure.Cosmos;

namespace CRM.Api.Services;

public class CosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(IConfiguration configuration)
    {
        var client = new CosmosClient(
            configuration["CosmosDb:Endpoint"],
            configuration["CosmosDb:Key"]);

        var database = client.CreateDatabaseIfNotExistsAsync(
            configuration["CosmosDb:DatabaseName"]).Result;

        var container = database.Database.CreateContainerIfNotExistsAsync(
            configuration["CosmosDb:ContainerName"],
            "/id").Result;

        _container = container.Container;
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        await _container.CreateItemAsync(customer, new PartitionKey(customer.Id));
    }
}