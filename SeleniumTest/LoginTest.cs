using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest
{
    [TestFixture]
    public class LoginTest
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();

            // Set implicit wait to 10 seconds
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        }

        [Test]
        public void TestLoginWithValidCredentials()
        {
            // Navigate to the login page
            driver.Navigate().GoToUrl("http://altoro.testfire.net/login.jsp");

            // Find the username and password fields and submit button
            IWebElement usernameField = driver.FindElement(By.Name("uid"));
            IWebElement passwordField = driver.FindElement(By.Name("passw"));
            IWebElement loginButton = driver.FindElement(By.Name("btnSubmit"));

            // Input valid credentials
            usernameField.SendKeys("admin");
            passwordField.SendKeys("admin");

            // Click the login button
            loginButton.Click();

            // If success page is found, assert the URL
            Assert.AreEqual("http://altoro.testfire.net/bank/main.jsp", driver.Url);
        }

        [Test]
        public void TestLoginWithInvalidCredentials()
        {
            // Navigate to the login page
            driver.Navigate().GoToUrl("http://altoro.testfire.net/login.jsp");

            // Find the username and password fields and submit button using CssSelector
            IWebElement usernameField = driver.FindElement(By.CssSelector("input[name='uid']"));
            IWebElement passwordField = driver.FindElement(By.CssSelector("input[name='passw']"));
            IWebElement loginButton = driver.FindElement(By.CssSelector("input[name='btnSubmit']"));

            // Input invalid credentials
            usernameField.SendKeys("invalid_user");
            passwordField.SendKeys("invalid_password");

            // Click the login button
            loginButton.Click();

            // Check for the presence of an error message
            IWebElement errorMessage = driver.FindElement(By.XPath("//*[contains(text(), 'Login Failed')]"));

            // If error message is found, assert the text
            Assert.AreEqual("Login Failed: We're sorry, but this username or password was not found in our system. Please try again.", errorMessage.Text);
        }

        [TearDown]
        public void Teardown()
        {
            // Close the browser window
            driver.Quit();
        }

    }
}
