using Microsoft.AspNetCore.Mvc;
using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;
using Raven.NET.Demo.WebApi.RequestModel;

namespace Raven.NET.Demo.WebApi.Controllers
{
//PS: This api is not an example of properly done SOLID Web Api, its simple showcase to see how Raven.NET components can be implemented
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderController(ILogger<OrderController> logger, IOrderRepository orderRepository,
            ICustomerRepository customerRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
        }

        // GET: api/Order
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            _logger.LogInformation("Get all orders invoked");
            var result = _orderRepository.GetAll();
            return result;
        }

        // GET: api/Order/8d35a21b-2b53-4407-8959-75910582af36
        [HttpGet("{id}")]
        public Order Get(Guid id)
        {
            _logger.LogInformation($"Get order {id} invoked");
            var result = _orderRepository.Get(id);
            return result;
        }

        // POST: api/Order
        [HttpPost]
        public IActionResult Post([FromBody] OrderCreateRequest value)
        {
            _logger.LogInformation($"Create order {value.Number} invoked");
            var customer = _customerRepository.Get(value.CustomerId);
            Order order = new Order(value.Number, customer, value.Product, value.Price);
            customer.OrderCount++;

            _orderRepository.Save(order);
            _customerRepository.Update(customer);
            return NoContent();
        }

        // PUT: api/Order/8d35a21b-2b53-4407-8959-75910582af36
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] OrderUpdateRequest value)
        {
            _logger.LogInformation($"Update order {id} invoked");
            var order = _orderRepository.Get(id);

            order.Price = value.Price;
            order.Product = value.Product;

            _orderRepository.Update(order);
            return Ok(order);
        }

        // DELETE: api/Order/8d35a21b-2b53-4407-8959-75910582af36
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _logger.LogInformation($"Delete order {id} invoked");
            _orderRepository.Delete(id);
            return NoContent();
        }
    }
}
