using Microsoft.EntityFrameworkCore.Internal;

namespace IntegrationSample
{
    public class CustomerBL
    {
        private IEntityRepository<Customer> _customerRepository { get; set; }
        public CustomerBL(IEntityRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public List<Customer> GetCustomers() { 
        var result = new List<Customer>();
            result = _customerRepository.GetAllQueryable()
                .Where(s=>s.IsDeleted == false).ToList();
          
            
           return result;
        }

        public bool insertCustomer(Customer customer) {
            var isAdded = false;
            try
            {
                _customerRepository.Insert(customer);
                isAdded = true;
            }
            catch (Exception ex) { 
            isAdded = false;
            }
            return isAdded; 
        }
    }
}
