using Raven.NET.Core.Observers;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;
using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Model.Enum;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;
using Raven.NET.Demo.WebApi.Services.Interfaces;

namespace Raven.NET.Demo.WebApi.Services
{
    public class OrderUpdateService : IOrderUpdateService
    {
        private readonly IRavenWatcher _ravenWatcher;
        private readonly ILogger<OrderUpdateService> logger;
        private IOrderRepository _orderRepository;
        private const string RavenName = "OrderWatcher";

        public OrderUpdateService(IRavenWatcher ravenWatcher, ILogger<OrderUpdateService> logger)
        {
            _ravenWatcher = ravenWatcher;
            this.logger = logger;
        }
        
        public void Initialize(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _ravenWatcher.Create(RavenName, Callback);
        }

        public void Track(Order order)
        {
            _ravenWatcher.Watch(order);
        }

        public void ProcessOrder(Order order)
        {
            if (order.Customer.OrderCount > 5)
            {
                order.Status = OrderStatus.Verified;
                order.TryNotify();
            }
            
            if (order.Customer.DiscountAvailable && order.Customer.DiscountPercentage > 0)
            {
                order.Price = order.Price * 100 / order.Customer.DiscountPercentage;
                order.TryNotify();
            }
        }

        private bool Callback(RavenSubject arg)
        {
            var order = arg as Order;
            logger.LogInformation($"Order {order.Number} updated");
            
            _orderRepository.Update(order);
            return true;
        }
    }
}