using Microsoft.AspNetCore.Mvc;
using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;
using Raven.NET.Demo.WebApi.RequestModel;

namespace Raven.NET.Demo.WebApi.Controllers
{
//PS: This api is not an example of properly done SOLID Web Api, its simple showcase to see how Raven.NET components can be implemented
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<OrderController> _logger;

        public CustomerController(ICustomerRepository customerRepository, ILogger<OrderController> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        // GET: api/Customer
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            _logger.LogInformation("Get all customers invoked");
            var result = _customerRepository.GetAll();
            return result;
        }

        // GET: api/Customer/8d35a21b-2b53-4407-8959-75910582af36
        [HttpGet("{id}")]
        public Customer Get(Guid id)
        {
            _logger.LogInformation($"Get customer {id} invoked");
            var result = _customerRepository.Get(id);
            return result;
        }

        // POST: api/Customer
        [HttpPost]
        public IActionResult Post([FromBody] CustomerCreateRequest customerCreateRequest)
        {
            _logger.LogInformation(
                $"Create customer {customerCreateRequest.FirstName} | {customerCreateRequest.LastName} invoked");
            var customer = new Customer(customerCreateRequest.FirstName, customerCreateRequest.LastName);
            _customerRepository.Save(customer);
            return NoContent();
        }

        // PUT: api/Customer/8d35a21b-2b53-4407-8959-75910582af36
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] CustomerDiscountRequest customerDiscount)
        {
            _logger.LogInformation($"Update customer {id} invoked");
            var customer = _customerRepository.Get(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (customerDiscount.CanHaveDiscount && customer.DiscountAvailable)
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

        // DELETE: api/Customer/8d35a21b-2b53-4407-8959-75910582af36
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _customerRepository.Delete(id);
            return NoContent();
        }
    }
}