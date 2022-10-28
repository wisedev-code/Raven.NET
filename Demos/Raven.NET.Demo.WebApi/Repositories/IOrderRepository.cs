using Raven.NET.Demo.WebApi.Model;

namespace Raven.NET.Demo.WebApi.Repositories;

public interface IOrderRepository
{
    public void Save(Order order);
    public void Update(Order order);
    public void Delete(Guid id);
    public Order Get(Guid id);
    public IEnumerable<Order> GetAll();
}