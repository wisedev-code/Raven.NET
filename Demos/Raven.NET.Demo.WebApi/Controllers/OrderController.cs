using Microsoft.AspNetCore.Mvc;
using Raven.NET.Demo.WebApi.RequestModel;

namespace Raven.NET.Demo.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger)
    {
        _logger = logger;
    }
    
    // GET: api/Order
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET: api/Order/5
    [HttpGet("{id}")]
    public string Get(Guid id)
    {
        return "value";
    }

    // POST: api/Order
    [HttpPost]
    public void Post([FromBody] OrderCreateRequest value)
    {
    }

    // PUT: api/Order/5
    [HttpPut("{id}")]
    public void Put(Guid id, [FromBody] OrderUpdateRequest value)
    {
    }

    // DELETE: api/Order/5
    [HttpDelete("{id}")]
    public void Delete(Guid id)
    {
    }
}
