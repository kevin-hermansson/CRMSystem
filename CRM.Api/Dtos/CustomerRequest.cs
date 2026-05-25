namespace CRM.Api.Dtos;

public record CustomerRequest(
    string Name,
    string Title,
    string Phone,
    string Email,
    string Address,
    SalesPersonDto SalesPerson);
