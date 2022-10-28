using Microsoft.AspNetCore.Mvc;
using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories;
using Raven.NET.Demo.WebApi.RequestModel;

namespace Raven.NET.Demo.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    // GET: api/Customer
    [HttpGet]
    public IEnumerable<Customer> Get()
    {
        var result = _customerRepository.GetAll();
        return result;
    }

    // GET: api/Customer/5
    [HttpGet("{id}")]
    public Customer Get(Guid id)
    {
        var result = _customerRepository.Get(id);
        return result;
    }

    // POST: api/Customer
    [HttpPost]
    public IActionResult Post([FromBody] CustomerCreateRequest customerCreateRequest)
    {
        var customer = new Customer(customerCreateRequest.FirstName, customerCreateRequest.LastName);
        _customerRepository.Save(customer);
        return NoContent();
    }

    // PUT: api/Customer/5
    [HttpPut("{id}")]
    public IActionResult Put(Guid id, [FromBody] CustomerDiscountRequest customerDiscount)
    {
        var customer = _customerRepository.Get(id);
        if (customer == null)
        {
            return NotFound();
        }

        if (customerDiscount.CanHaveDiscount)
        {
            customer.GiveDiscount(customerDiscount.Discount);
        }
        else
        {
            customer.RemoveDiscount();
        }

        _customerRepository.Update(customer);

        return Ok(customer);
    }

    // DELETE: api/Customer/5
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _customerRepository.Delete(id);
        return NoContent();
    }
}