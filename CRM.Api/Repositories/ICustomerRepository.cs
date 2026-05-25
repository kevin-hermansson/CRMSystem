using CRM.Api.Models;

namespace CRM.Api.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task AddAsync(Customer customer);
    Task<List<Customer>> SearchAsync(string search);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(string id);
}
