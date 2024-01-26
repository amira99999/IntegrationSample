using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IntegrationSample.Tests
{
    public class EntityRepositoryTests
    {
        [Test]
        public void GetAllQueryable_WhenCalled_ReturnsQueryable()
        {
            // Arrange
            // Creating options for an in-memory database
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            // Mocking CustomerDbContext with the in-memory database options
            var dbContextMock = new Mock<CustomerDbContext>(options);
            var dbSetMock = new Mock<DbSet<Customer>>();

            // Setting up the Set method of CustomerDbContext to return the DbSet mock
            dbContextMock.Setup(dbContext => dbContext.Set<Customer>())
                .Returns(dbSetMock.Object);

            // Creating an instance of the EntityRepository with the mocked CustomerDbContext
            var repository = new EntityRepository<Customer>(dbContextMock.Object);

            // Act
            var result = repository.GetAllQueryable();

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<IQueryable<Customer>>(result);  // Ensure the result is of type IQueryable<Customer>
        }

        [Test]
        public void Insert_ShouldAddEntityToDbSetAndSaveChanges()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContextMock = new Mock<CustomerDbContext>(dbContextOptions);
            var dbSetMock = new Mock<DbSet<Customer>>();

            dbContextMock.Setup(dbContext => dbContext.Set<Customer>())
                .Returns(dbSetMock.Object);

            var repository = new EntityRepository<Customer>(dbContextMock.Object);

            // Creating a sample entity to add
            var entityToAdd = new Customer { Id = 1, FirstName = "John", LastName = "Doe", CustomerId = "ABC01", IsDeleted = false };

            // Act
            repository.Insert(entityToAdd);

            // Assert
            dbSetMock.Verify(set => set.Add(entityToAdd), Times.Once);  // Ensure the Add method of DbSet is called once with the provided entity
            dbContextMock.Verify(dbContext => dbContext.SaveChanges(), Times.Once); // Ensure the SaveChanges method of CustomerDbContext is called once
        }
    }
}
