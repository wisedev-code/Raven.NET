namespace Raven.NET.Demo.WebApi.RequestModel;

public class OrderCreateRequest
{
    public string Number { get; set; }
    public Guid CustomerId { get; set; }
    public string Product { get; set; }
    public decimal Price { get; set; }
}