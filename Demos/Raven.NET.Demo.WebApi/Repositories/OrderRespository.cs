using System.Collections.Concurrent;
using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;
using Raven.NET.Demo.WebApi.Services.Interfaces;

namespace Raven.NET.Demo.WebApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<Guid, Order> _orders = new();
        private readonly IOrderUpdateService _orderUpdateService;

        public OrderRepository(IOrderUpdateService orderUpdateService)
        {
            _orderUpdateService = orderUpdateService;
            _orderUpdateService.Initialize(this);
        }

        public void Save(Order order)
        {
            _orders.TryAdd(order.Id, order);
        }

        public void Update(Order order)
        {
            if (!_orders.ContainsKey(order.Id))
            {
                throw new KeyNotFoundException();
            }

            _orders[order.Id] = order;
        }

        public void Delete(Guid id)
        {
            if (!_orders.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }

            _orders.Remove(id, out _);
        }

        public Order Get(Guid id)
        {
            if (!_orders.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }

            return _orders[id];
        }

        public IEnumerable<Order> GetAll()
        {
            return _orders.Values;
        }
    }
}