using CRM.Api.Models;

namespace CRM.Api.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task AddCustomerAsync(Customer customer);
    Task<List<Customer>> SearchCustomersAsync(string search);
    Task UpdateCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(string id);
}
