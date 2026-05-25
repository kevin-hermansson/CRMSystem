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

        group.MapGet("", async (CosmosDbService db) =>
        {
            var customers = await db.GetCustomersAsync();
            return Results.Ok(customers.Select(ToResponse));
        });

        group.MapPost("", async (CustomerRequest request, CosmosDbService db) =>
        {
            var customer = ToCustomer(request);
            await db.AddCustomerAsync(customer);

            return Results.Ok(ToResponse(customer));
        });

        group.MapGet("/search", async (string search, CosmosDbService db) =>
        {
            var customers = await db.SearchCustomersAsync(search);
            return Results.Ok(customers.Select(ToResponse));
        });

        group.MapPut("/{id}", async (string id, CustomerRequest request, CosmosDbService db) =>
        {
            var updatedCustomer = ToCustomer(request, id);
            await db.UpdateCustomerAsync(updatedCustomer);

            return Results.Ok(ToResponse(updatedCustomer));
        });

        group.MapDelete("/{id}", async (string id, CosmosDbService db) =>
        {
            await db.DeleteCustomerAsync(id);
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
        return new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Title = customer.Title,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            SalesPerson = new SalesPersonDto
            {
                Name = customer.SalesPerson.Name,
                Phone = customer.SalesPerson.Phone,
                Email = customer.SalesPerson.Email
            }
        };
    }
}
