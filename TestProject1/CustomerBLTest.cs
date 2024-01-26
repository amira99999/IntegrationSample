using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationSample.Tests
{
    public class CustomerBLTest
    {
        [Test]
        public void GetCustomers_WhenCalled_ReturnsCustomersList()
        {
            // Arrange
            var customerRepositoryMock = new Mock<IEntityRepository<Customer>>();
            customerRepositoryMock.Setup(repo => repo.GetAllQueryable())
                .Returns(new List<Customer>
                {
                new Customer { Id = 1, FirstName = "John", LastName = "Doe", CustomerId = "ABC01", IsDeleted = false },
                new Customer { Id = 2, FirstName = "Jahn", LastName = "Dae", CustomerId = "ABC02", IsDeleted = false }

                }.AsQueryable());

            var customerBL = new CustomerBL(customerRepositoryMock.Object);

            // Act
            var result = customerBL.GetCustomers();

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.AreEqual(2, result.Count);   // Ensure the correct number of customers is returned
        }

        [Test]
        public void GetCustomers_ShouldReturnNonDeletedCustomers()
        {
            // Arrange
            var customerRepositoryMock = new Mock<IEntityRepository<Customer>>();
            customerRepositoryMock.Setup(repo => repo.GetAllQueryable())
                .Returns(new List<Customer>
                {
                new Customer { Id = 1, FirstName = "John", LastName = "Smith", CustomerId = "ABC01", IsDeleted = false },
                new Customer { Id = 2, FirstName = "Emily", LastName = "Johnson", CustomerId = "ABC02" ,IsDeleted = true },
                new Customer { Id = 3, FirstName = "Michael", LastName = "Rodriguez", CustomerId = "ABC03" ,IsDeleted = false },
                }.AsQueryable());

            var customerBL = new CustomerBL(customerRepositoryMock.Object);

            // Act
            var result = customerBL.GetCustomers();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // Two non-deleted customers
            result.Should().OnlyContain(c => c.IsDeleted == false);
        }

        [Test]
        public void InsertCustomer_WhenCalled_ReturnsTrue()
        {
            // Arrange
            var customerRepositoryMock = new Mock<IEntityRepository<Customer>>();
            var customerBL = new CustomerBL(customerRepositoryMock.Object);
            var customer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", CustomerId = "ABC01", IsDeleted = false }
;

            // Act
            var isAdded = customerBL.insertCustomer(customer);

            // Assert
            Assert.True(isAdded);   // Ensure the customer is successfully added
            customerRepositoryMock.Verify(repo => repo.Insert(customer), Times.Once);   // Ensure the repository's Insert method is called once
        }

        [Test]
        public void InsertCustomer_ShouldHandleExceptionAndReturnFalse()
        {
            // Arrange
            var customerRepositoryMock = new Mock<IEntityRepository<Customer>>();
            customerRepositoryMock.Setup(repo => repo.Insert(It.IsAny<Customer>()))
                .Throws(new Exception("Simulated exception"));

            var customerBL = new CustomerBL(customerRepositoryMock.Object);

            var customer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", CustomerId = "ABC01", IsDeleted = false };

            // Act
            var isAdded = customerBL.insertCustomer(customer);

            // Assert
            // Ensure the customer is not added
            isAdded.Should().BeFalse();
            customerRepositoryMock.Verify(repo => repo.Insert(customer), Times.Once);
        }

    }
}
