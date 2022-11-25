using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;

namespace Raven.NET.Demo.WebApi.Services.Interfaces
{
    public interface IOrderUpdateService
    {
        void Initialize(IOrderRepository orderRepository);
        void Track(Order order);
        void ProcessOrder(Order order);
    }
}