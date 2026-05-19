using CRM.Api.Models;
using CRM.Api.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CosmosDbService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//GetAll
app.MapGet("/customers", async (CosmosDbService cosmosDbService) =>
{
    var customers = await cosmosDbService.GetCustomersAsync();
    return Results.Ok(customers);
});
//Create
app.MapPost("/customers", async (Customer customer, CosmosDbService cosmosDbService) =>
{
    customer.Id = Guid.NewGuid().ToString();
    await cosmosDbService.AddCustomerAsync(customer);

    return Results.Ok(customer);
});
//Search
app.MapGet("/customers/search", async (string search, CosmosDbService cosmosDbService) =>
{
    var customers = await cosmosDbService.SearchCustomersAsync(search);
    return Results.Ok(customers);
});
//Update
app.MapPut("/customers/{id}", async (string id, Customer updatedCustomer, CosmosDbService cosmosDbService) =>
{
    updatedCustomer.Id = id;
    await cosmosDbService.UpdateCustomerAsync(updatedCustomer);
    return Results.Ok(updatedCustomer);
});
//Delete
app.MapDelete("/customers/{id}", async (string id, CosmosDbService cosmosDbService) =>
{
    await cosmosDbService.DeleteCustomerAsync(id);
    return Results.Ok();
});

app.UseHttpsRedirection();



app.Run();

   