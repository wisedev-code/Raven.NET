using Raven.NET.Demo.WebApi.Repositories.Interfaces;

namespace Raven.NET.Demo.WebApi.Services.Interfaces
{
    public interface ICustomerUpdateService
    {
        void Initialize(ICustomerRepository customerRepository);
    }
}