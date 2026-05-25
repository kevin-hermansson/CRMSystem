namespace CRM.Api.Dtos;

public record CustomerResponse(
    string Id,
    string Name,
    string Title,
    string Phone,
    string Email,
    string Address,
    SalesPersonDto SalesPerson);
