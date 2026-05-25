namespace CRM.Api.Dtos;

public class CustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public SalesPersonDto SalesPerson { get; set; } = new SalesPersonDto();
}
