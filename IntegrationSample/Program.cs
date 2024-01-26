using FluentAssertions.Common;
using IntegrationSample;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CustomerDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: "TestDatabase"));
builder.Services.AddScoped<IEntityRepository<Customer>, EntityRepository<Customer>>();
builder.Services.AddScoped<CustomerBL>();

builder.Services.AddRouting(); // Add routing services

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); // Use routing

app.MapPost("/insertCustomer", async context =>
{
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

app.MapGet("/getCustomers", async context =>
{
    var customerBL = context.RequestServices.GetRequiredService<CustomerBL>();
    var customers = customerBL.GetCustomers();
    var response = string.Join(", ", customers.Select(c => $"{c.FirstName} {c.LastName}"));
    await context.Response.WriteAsync(response);
});

app.Run();

public partial class Program { }
