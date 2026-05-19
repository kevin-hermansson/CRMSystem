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

    public async Task<List<Customer>> GetCustomersAsync()
    {
        var query = _container.GetItemQueryIterator<Customer>(
            new QueryDefinition("SELECT * FROM c"));

        List<Customer> customers = new();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            customers.AddRange(response.ToList());
        }

        return customers;
    }
    public async Task AddCustomerAsync(Customer customer)
    {
        await _container.CreateItemAsync(customer, new PartitionKey(customer.Id));
    }

    public async Task<List<Customer>> SearchCustomersAsync(string search)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE CONTAINS(c.name, @search) OR CONTAINS(c.salesPerson.name, @search)")
            .WithParameter("@search", search);

        var iterator = _container.GetItemQueryIterator<Customer>(query);

        List<Customer> customers = new();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();

            customers.AddRange(response);
        }

        return customers;
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        await _container.UpsertItemAsync(customer, new PartitionKey(customer.Id));
    }
    public async Task DeleteCustomerAsync(string id)
    {
            await _container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
    }
}
