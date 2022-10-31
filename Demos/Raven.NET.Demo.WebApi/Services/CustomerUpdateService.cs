using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;
using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;
using Raven.NET.Demo.WebApi.Services.Interfaces;

namespace Raven.NET.Demo.WebApi.Services
{
    public class CustomerUpdateService : ICustomerUpdateService
    {
        private readonly IRavenTypeWatcher _ravenTypeWatcher;
        private readonly ILogger<CustomerUpdateService> logger;
        private ICustomerRepository _customerRepository;
        private const string RavenName = "CustomerWatcher";

        public CustomerUpdateService(IRavenTypeWatcher ravenTypeWatcher, ILogger<CustomerUpdateService> logger)
        {
            _ravenTypeWatcher = ravenTypeWatcher;
            this.logger = logger;
        }
        
        public void Initialize(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _ravenTypeWatcher.Create<Customer>(RavenName, nameof(Customer.Id), Callback);
        }

        private bool Callback(RavenSubject arg)
        {
            var customer = arg as Customer;
            logger.LogInformation($"Customer callback changed invoked {customer.Id}");

            if (customer.OrderCount >= 10)
            {
                customer.DiscountAvailable = true;
            }
            
            _customerRepository.Update(customer);

            return true;
        }
    }
}