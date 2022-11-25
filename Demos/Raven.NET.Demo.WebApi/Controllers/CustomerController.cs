using Microsoft.AspNetCore.Mvc;
using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;
using Raven.NET.Demo.WebApi.RequestModel;
using Swashbuckle.AspNetCore.Annotations;

namespace Raven.NET.Demo.WebApi.Controllers
{
    //PS: This api is not an example of properly done SOLID Web Api, its simple showcase to see how Raven.NET components can be implemented
    [SwaggerTag("Controller to work with Customers (supported by RavenTypeWatcher")]
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
        [SwaggerOperation(Summary = "Get all available customers")]
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            _logger.LogInformation("Get all customers invoked");
            var result = _customerRepository.GetAll();
            return result;
        }

        // GET: api/Customer/8d35a21b-2b53-4407-8959-75910582af36
        [SwaggerOperation("Get customer by id")]
        [HttpGet("{id}")]
        public Customer Get(Guid id)
        {
            _logger.LogInformation($"Get customer {id} invoked");
            var result = _customerRepository.Get(id);
            return result;
        }

        // POST: api/Customer
        [SwaggerOperation("Create new customer (will as well invoke RavenTypeWatcher processing")]
        [HttpPost]
        public IActionResult Post([FromBody] CustomerCreateRequest customerCreateRequest)
        {
            _logger.LogInformation($"Create customer {customerCreateRequest.FirstName} | {customerCreateRequest.LastName} invoked");
            var customer = new Customer(customerCreateRequest.FirstName, customerCreateRequest.LastName);
            _customerRepository.Save(customer);
            return NoContent();
        }

        // PUT: api/Customer/8d35a21b-2b53-4407-8959-75910582af36
        [SwaggerOperation("Update existing customer - You can update customer by adding discount or removing existing one. Adding discount will only be possible if customer make already at least 10 orders")]
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
        [SwaggerOperation(Summary = "Delete customer by id")]
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _customerRepository.Delete(id);
            return NoContent();
        }
    }
}