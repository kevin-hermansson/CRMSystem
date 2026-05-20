using Newtonsoft.Json;
namespace CRM.Functions.Models
{
    public class Customer
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public SalesPerson SalesPerson { get; set; } = new SalesPerson();
    }
}
