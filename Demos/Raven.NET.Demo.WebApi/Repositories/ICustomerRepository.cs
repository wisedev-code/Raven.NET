using Raven.NET.Demo.WebApi.Model;

namespace Raven.NET.Demo.WebApi.Repositories;

public interface ICustomerRepository
{
    public void Save(Customer customer);
    public void Update(Customer customer);
    public void Delete(Guid id);
    public Customer Get(Guid id);
    public IEnumerable<Customer> GetAll();
}