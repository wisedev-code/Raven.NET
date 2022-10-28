namespace Raven.NET.Demo.WebApi.RequestModel;

public class CustomerDiscountRequest
{
    public bool CanHaveDiscount { get; set; }
    public int Discount { get; set; }
}