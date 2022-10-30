using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;

namespace Raven.NET.Demo.WebApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Dictionary<Guid, Order> _orders = new();
        public void Save(Order order)
        {
            _orders.Add(order.Id, order);
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

            _orders.Remove(id);
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