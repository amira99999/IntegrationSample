using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegrationSample.Tests
{
    public class IntegrationTests
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            // Configure the test server
            var hostBuilder = new WebHostBuilder()
             .UseEnvironment("Development")
             .ConfigureServices(services =>
             {
                 // Add necessary services
                 services.AddControllers();
                 services.AddEndpointsApiExplorer();
                 services.AddSwaggerGen();
                 services.AddDbContext<CustomerDbContext>(options =>
                     options.UseInMemoryDatabase(databaseName: "TestDatabase"));
                 services.AddScoped<IEntityRepository<Customer>, EntityRepository<Customer>>();
                 services.AddScoped<CustomerBL>();
                 services.AddRouting(); // Add routing services
             })
             .Configure(app =>
             {
                 app.UseRouting();

                 // Configure endpoints for testing
                 app.UseEndpoints(endpoints =>
                 {
                     endpoints.MapPost("/insertCustomer", async context =>
                     {
                         // Handle insertCustomer endpoint
                         var customerBL = context.RequestServices.GetRequiredService<CustomerBL>();
                         var customer = new Customer
                         {
                             Id = 1,
                             FirstName = "John",
                             LastName = "Doe",
                             CustomerId = "ABC01",
                             IsDeleted = false
                         };
                         customerBL.insertCustomer(customer);
                         await context.Response.WriteAsync("Customer inserted successfully.");
                     });

                     endpoints.MapGet("/getCustomers", async context =>
                     {
                         // Handle getCustomers endpoint
                         var customerBL = context.RequestServices.GetRequiredService<CustomerBL>();
                         var customers = customerBL.GetCustomers();
                         var response = JsonSerializer.Serialize(customers);
                         await context.Response.WriteAsync(response);
                     });
                 });
             });

            // Create a test server and client
            var testServer = new TestServer(hostBuilder);
            _client = testServer.CreateClient();
        }

        [Test]
        public async Task InsertCustomer_EndpointIntegrationTest()
        {
            // Arrange
            var requestContent = new StringContent(string.Empty);

            // Act
            var response = await _client.PostAsync("/insertCustomer", requestContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Customer inserted successfully.", responseContent);
        }

        [Test]
        public async Task GetCustomers_EndpointIntegrationTest()
        {
            // Arrange (insert a customer first)
            var insertRequestContent = new StringContent(string.Empty);
            await _client.PostAsync("/insertCustomer", insertRequestContent);

            // Act
            var response = await _client.GetAsync("/getCustomers");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Adjust the assertion based on the actual response format
            var expectedResponse = "[{\"Id\":1,\"FirstName\":\"John\",\"LastName\":\"Doe\",\"CustomerId\":\"ABC01\",\"IsDeleted\":false}]";
            Assert.AreEqual(expectedResponse, responseContent);
        }

        [Test]
        public async Task InsertAndGetCustomers_EndpointsIntegrationTest()
        {
            // Insert a customer
            var insertResponse = await _client.PostAsync("/insertCustomer", null);
            insertResponse.EnsureSuccessStatusCode();

            // Get customers
            var getResponse = await _client.GetAsync("/getCustomers");
            getResponse.EnsureSuccessStatusCode();

            // Deserialize the response
            var customers = await JsonSerializer.DeserializeAsync<List<Customer>>(await getResponse.Content.ReadAsStreamAsync());

            // Assert
            Assert.NotNull(customers);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual("John", customers[0].FirstName);
            Assert.AreEqual("Doe", customers[0].LastName);
        }
    }
}
