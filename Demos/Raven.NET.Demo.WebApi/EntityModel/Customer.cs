using System.Runtime.CompilerServices;

namespace Raven.NET.Demo.WebApi.Model;

public class Customer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool DiscountAvailable { get; set; }
    public int DiscountPercentage { get; set; }

    public Customer(string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
    }

    public void GiveDiscount(int percentage)
    {
        DiscountAvailable = true;
        DiscountPercentage = percentage;
    }

    public void RemoveDiscount()
    {
        DiscountAvailable = false;
        DiscountPercentage = 0;
    }
}