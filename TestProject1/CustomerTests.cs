using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationSample.Tests
{
    public class CustomerTests
    {
        [Test]
        public void Customer_ShouldHaveValidProperties()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                CustomerId = "ABC01",
                IsDeleted = false
            };

            // Act
            var validationResults = new List<ValidationResult>();
            // Validate the customer object using the TryValidateObject method
            var isValid = Validator.TryValidateObject(customer, new ValidationContext(customer), validationResults, validateAllProperties: true);

            // Assert
            isValid.Should().BeTrue();
            // Ensure that there are no validation results (no errors)
            validationResults.Should().BeEmpty();
        }

        [Test]
        public void Customer_ShouldFailValidationWithInvalidProperties()
        {
            // Arrange
            var customer = new Customer
            {
                // Invalid: FirstName is null (violates [Required])
                Id = 1,
                FirstName = null,
                LastName = "Doe",
                CustomerId = "ABC01",
                IsDeleted = false
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(customer, new ValidationContext(customer), validationResults, validateAllProperties: true);

            // Assert
            // Ensure that validation fails
            isValid.Should().BeFalse();
            // Ensure that there are validation results (errors present)
            validationResults.Should().NotBeEmpty();
        }
    }
}
