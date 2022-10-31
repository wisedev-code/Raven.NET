using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Subjects;
using Raven.NET.Demo.WebApi.Model;
using Raven.NET.Demo.WebApi.Repositories.Interfaces;
using Raven.NET.Demo.WebApi.Services;
using Raven.NET.Demo.WebApi.Services.Interfaces;

namespace Raven.NET.Demo.WebApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly Dictionary<Guid, Customer> _customers = new();
        private readonly ICustomerUpdateService _customerUpdateService;

        public CustomerRepository(ICustomerUpdateService customerUpdateService)
        {
            _customerUpdateService = customerUpdateService;
            _customerUpdateService.Initialize(this);
        }

        public void Save(Customer customer)
        {
            _customers.Add(customer.Id, customer);
        }

        public void Update(Customer customer)
        {
            if (!_customers.ContainsKey(customer.Id))
            {
                throw new KeyNotFoundException();
            }

            _customers[customer.Id] = customer;
        }

        public void Delete(Guid id)
        {
            if (!_customers.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }

            _customers.Remove(id);
        }

        public Customer Get(Guid id)
        {
            if (!_customers.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }

            return _customers[id];
        }

        public IEnumerable<Customer> GetAll()
        {
            return _customers.Values;
        }
    }
}