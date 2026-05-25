# CRM System

A small .NET 8 CRM backend for storing customers, assigning sales people, and sending notifications when customer records change.

## Projects

| Project | Description |
| --- | --- |
| `CRM.Api` | ASP.NET Core minimal API for customer CRUD, search, Swagger, and Cosmos DB persistence. |
| `CRM.Functions` | Azure Functions isolated worker app with a Cosmos DB trigger for customer-change notifications. |

## Tech Stack

- .NET 8
- ASP.NET Core minimal APIs
- Azure Cosmos DB
- Azure Functions v4 isolated worker
- Swagger / OpenAPI

## Prerequisites

- .NET 8 SDK
- Azure Cosmos DB account, or the Cosmos DB emulator
- Azure Functions Core Tools, if running `CRM.Functions` locally

## Configuration

### API

Configure Cosmos DB settings in `CRM.Api/appsettings.json`, user secrets, or environment variables.

```json
{
  "CosmosDb": {
    "Endpoint": "https://your-account.documents.azure.com:443/",
    "Key": "your-cosmos-key",
    "DatabaseName": "CRMDatabase",
    "ContainerName": "Customers"
  }
}
```

The API creates the configured database and container if they do not already exist. The container partition key is `/id`.

### Azure Function

Create `CRM.Functions/local.settings.json` for local development.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDbConnection": "AccountEndpoint=https://your-account.documents.azure.com:443/;AccountKey=your-cosmos-key;"
  }
}
```

The function listens to:

- Database: `CRMDatabase`
- Container: `Customers`
- Lease container: `leases`

## Run Locally

Restore and build the solution:

```powershell
dotnet restore
dotnet build
```

Run the API:

```powershell
dotnet run --project CRM.Api
```

Default local URLs:

- Swagger: `https://localhost:7159/swagger`
- HTTP API: `http://localhost:5212`

Run the Azure Function:

```powershell
cd CRM.Functions
func start --port 7125
```

## API Endpoints

Base route: `/customers`

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/customers` | Get all customers. |
| `GET` | `/customers/search?search={term}` | Search by customer name or sales person name. |
| `POST` | `/customers` | Create a customer. |
| `PUT` | `/customers/{id}` | Update a customer. |
| `DELETE` | `/customers/{id}` | Delete a customer. |

## Customer Request Example

```json
{
  "name": "Contoso Ltd",
  "title": "Procurement Manager",
  "phone": "+1 555 0100",
  "email": "buyer@contoso.com",
  "address": "1 Microsoft Way, Redmond, WA",
  "salesPerson": {
    "name": "Alex Morgan",
    "phone": "+1 555 0199",
    "email": "alex.morgan@example.com"
  }
}
```

## Notes

- Do not commit real Cosmos DB keys or SMTP credentials.
- Keep local secrets in `appsettings.Development.json`, user secrets, or `local.settings.json`.
- The notification function currently uses a Mailtrap SMTP sandbox and should be moved to configuration before production use.
