namespace Raven.NET.Demo.WebApi.Model;

public class Order
{
    public Guid Id { get; set; }
    public string Number { get; set; }
    public Customer Customer { get; set; }
    public string Product { get; set; }
    public decimal Price { get; set; }

    public Order(Guid id, string number, Customer customer, string product, decimal price)
    {
        Id = id;
        Number = number;
        Customer = customer;
        Product = product;
        Price = price;
    }
}