using CRM.Api.Dtos;
using CRM.Api.Models;
using CRM.Api.Services;

namespace CRM.Api.Endpoints;

public static class CustomerEndpoints
{
    public static RouteGroupBuilder MapCustomerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/customers")
            .WithTags("Customers");

        group.MapGet("", async (ICustomerService customerService) =>
        {
            var customers = await customerService.GetAllCustomersAsync();
            return Results.Ok(customers.Select(ToResponse));
        });

        group.MapPost("", async (CustomerRequest request, ICustomerService customerService) =>
        {
            var customer = ToCustomer(request);
            await customerService.AddCustomerAsync(customer);

            return Results.Ok(ToResponse(customer));
        });

        group.MapGet("/search", async (string search, ICustomerService customerService) =>
        {
            var customers = await customerService.SearchCustomersAsync(search);
            return Results.Ok(customers.Select(ToResponse));
        });

        group.MapPut("/{id}", async (string id, CustomerRequest request, ICustomerService customerService) =>
        {
            var updatedCustomer = ToCustomer(request, id);
            await customerService.UpdateCustomerAsync(updatedCustomer);

            return Results.Ok(ToResponse(updatedCustomer));
        });

        group.MapDelete("/{id}", async (string id, ICustomerService customerService) =>
        {
            await customerService.DeleteCustomerAsync(id);
            return Results.Ok();
        });

        return group;
    }

    private static Customer ToCustomer(CustomerRequest request, string? id = null)
    {
        return new Customer
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = request.Name,
            Title = request.Title,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            SalesPerson = new SalesPerson
            {
                Name = request.SalesPerson.Name,
                Phone = request.SalesPerson.Phone,
                Email = request.SalesPerson.Email
            }
        };
    }

    private static CustomerResponse ToResponse(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.Title,
            customer.Phone,
            customer.Email,
            customer.Address,
            new SalesPersonDto(
                customer.SalesPerson.Name,
                customer.SalesPerson.Phone,
                customer.SalesPerson.Email));
    }
}
