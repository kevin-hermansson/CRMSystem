using CRM.Api.Models;
using CRM.Api.Services;

namespace CRM.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CosmosDbService _cosmosDbService;

    public CustomerRepository(CosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public Task<List<Customer>> GetAllAsync() => _cosmosDbService.GetCustomersAsync();

    public Task AddAsync(Customer customer) => _cosmosDbService.AddCustomerAsync(customer);

    public Task<List<Customer>> SearchAsync(string search) => _cosmosDbService.SearchCustomersAsync(search);

    public Task UpdateAsync(Customer customer) => _cosmosDbService.UpdateCustomerAsync(customer);

    public Task DeleteAsync(string id) => _cosmosDbService.DeleteCustomerAsync(id);
}
