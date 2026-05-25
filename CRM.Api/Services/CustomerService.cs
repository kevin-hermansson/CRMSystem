using CRM.Api.Models;
using CRM.Api.Repositories;

namespace CRM.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Customer>> GetAllCustomersAsync() => _repository.GetAllAsync();

    public Task AddCustomerAsync(Customer customer) => _repository.AddAsync(customer);

    public Task<List<Customer>> SearchCustomersAsync(string search) => _repository.SearchAsync(search);

    public Task UpdateCustomerAsync(Customer customer) => _repository.UpdateAsync(customer);

    public Task DeleteCustomerAsync(string id) => _repository.DeleteAsync(id);
}
