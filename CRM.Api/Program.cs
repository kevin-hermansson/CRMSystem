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
app.MapPost("/customers", async (Customer customer, CosmosDbService cosmosDbService) =>
{
    customer.Id = Guid.NewGuid().ToString();
    await cosmosDbService.AddCustomerAsync(customer);

    return Results.Ok(customer);
});

app.UseHttpsRedirection();



app.Run();

   