using Microsoft.AspNetCore.Mvc;
using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;
using Raven.NET.Demo.WebApi.RequestModel;
using Raven.NET.Demo.WebApi.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Raven.NET.Demo.WebApi.Controllers
{
    //PS: This api is not an example of properly done SOLID Web Api, its simple showcase to see how Raven.NET components can be implemented
    [SwaggerTag("Controller to work with Orders (supported by RavenWatcher)")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderUpdateService _orderUpdateService;

        public OrderController(ILogger<OrderController> logger, IOrderRepository orderRepository,
            ICustomerRepository customerRepository, IOrderUpdateService orderUpdateService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _orderUpdateService = orderUpdateService;
        }

        // GET: api/Order
        [SwaggerOperation(Summary = "Get all available orders")]
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            _logger.LogInformation("Get all orders invoked");
            var result = _orderRepository.GetAll();
            return result;
        }

        // GET: api/Order/8d35a21b-2b53-4407-8959-75910582af36
        [SwaggerOperation(Summary = "Get order by id")]
        [HttpGet("{id}")]
        public Order Get(Guid id)
        {
            _logger.LogInformation($"Get order {id} invoked");
            var result = _orderRepository.Get(id);
            return result;
        }

        // POST: api/Order
        [SwaggerOperation(Summary = "Add order to in memory database (this will also include RavenWatcher tracking of order")]
        [HttpPost]
        public IActionResult Post([FromBody] OrderCreateRequest value)
        {
            _logger.LogInformation($"Create order {value.Number} invoked");
            var customer = _customerRepository.Get(value.CustomerId);
            Order order = new Order(value.Number, customer, value.Product, value.Price);
            customer.OrderCount++;
            _orderRepository.Save(order);
            _customerRepository.Update(customer);
            
            //We can add additional logic there for example to keep track only on specified orders.
            _orderUpdateService.Track(order);
            _orderUpdateService.ProcessOrder(order);
            
            return NoContent();
        }

        // PUT: api/Order/8d35a21b-2b53-4407-8959-75910582af36
        [SwaggerOperation(Summary = "Update order by given id (you can update only price and product")]
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
        [SwaggerOperation(Summary = "Delete order by given id")]
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _logger.LogInformation($"Delete order {id} invoked");
            _orderRepository.Delete(id);
            return NoContent();
        }
    }
}
