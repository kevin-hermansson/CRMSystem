using CRM.Api.Dtos;
using CRM.Api.Models;
using CRM.Api.Services;
using Microsoft.Azure.Cosmos;
using System.ComponentModel.DataAnnotations;

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
            var validationProblem = Validate(request);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var customer = ToCustomer(request);
            await db.AddCustomerAsync(customer);

            return Results.Created($"/customers/{customer.Id}", ToResponse(customer));
        });

        group.MapGet("/search", async (string search, CosmosDbService db) =>
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return Results.BadRequest(new { error = "Search query is required." });
            }

            var customers = await db.SearchCustomersAsync(search);
            return Results.Ok(customers.Select(ToResponse));
        });

        group.MapPut("/{id}", async (string id, CustomerRequest request, CosmosDbService db) =>
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Results.BadRequest(new { error = "Customer id is required." });
            }

            var validationProblem = Validate(request);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var updatedCustomer = ToCustomer(request, id);

            try
            {
                await db.UpdateCustomerAsync(updatedCustomer);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Results.NotFound(new { error = $"Customer with id '{id}' was not found." });
            }

            return Results.Ok(ToResponse(updatedCustomer));
        });

        group.MapDelete("/{id}", async (string id, CosmosDbService db) =>
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Results.BadRequest(new { error = "Customer id is required." });
            }

            try
            {
                await db.DeleteCustomerAsync(id);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Results.NotFound(new { error = $"Customer with id '{id}' was not found." });
            }

            return Results.NoContent();
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

    private static IResult? Validate(CustomerRequest request)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);

        Validator.TryValidateObject(request, context, results, validateAllProperties: true);

        if (request.SalesPerson is not null)
        {
            var salesPersonResults = new List<ValidationResult>();
            var salesPersonContext = new ValidationContext(request.SalesPerson);

            Validator.TryValidateObject(
                request.SalesPerson,
                salesPersonContext,
                salesPersonResults,
                validateAllProperties: true);

            results.AddRange(salesPersonResults.Select(result => new ValidationResult(
                result.ErrorMessage,
                result.MemberNames.Select(memberName => $"{nameof(CustomerRequest.SalesPerson)}.{memberName}"))));
        }

        if (results.Count == 0)
        {
            return null;
        }

        var errors = results
            .SelectMany(result => result.MemberNames.DefaultIfEmpty(string.Empty), (result, memberName) => new
            {
                MemberName = memberName,
                ErrorMessage = result.ErrorMessage ?? "The request is invalid."
            })
            .GroupBy(error => error.MemberName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        return Results.ValidationProblem(errors);
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
