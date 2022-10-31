using System.Runtime.CompilerServices;
using Raven.NET.Core.Subjects;

namespace Raven.NET.Demo.WebApi.Model;

public class Customer : RavenSubject
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool DiscountAvailable { get; set; }
    public int DiscountPercentage { get; set; }
    public int OrderCount { get; set; }

    public Customer(string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
    }

    public void GiveDiscount(int percentage)
    {
        DiscountPercentage = percentage;
    }

    public void RemoveDiscount()
    {
        DiscountAvailable = false;
        DiscountPercentage = 0;
    }
}