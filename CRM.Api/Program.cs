using CRM.Api.Endpoints;
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

app.UseHttpsRedirection();
app.MapCustomerEndpoints();

app.Run();
