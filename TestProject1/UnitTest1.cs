using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ReCVEServer.Controllers;
using ReCVEServer.Data;
using ReCVEServer.Models;
using ReCVEServer.NistApi;
using System.Collections.Generic;

namespace TestProject1 {

        [TestClass]
        public class AutomatedUITests : IDisposable {

       

    private readonly IWebDriver driver;
        public AutomatedUITests()
        {
            var chromeOptions = new ChromeOptions();
            if (Environment.GetEnvironmentVariable("CI") == "true")
            {
                chromeOptions.AddArguments("--headless", "--no-sandbox", "--disable-gpu");
            }
            driver = new ChromeDriver();

        }
            public void Dispose() {
                driver.Quit();
                driver.Dispose();


            }

        [TestMethod]
         public void FirstTest() {

             driver.Navigate()
                 .GoToUrl("https://localhost:7025/");

             Assert.AreEqual("Dashboard - ReCVEServer", driver.Title);
             
         }

        [TestMethod]
        public void ClickCveButton() {
            driver.Url = "https://localhost:7025/";
            IWebElement devicesButton = driver.FindElement(By.Id("cveBtn"));
            devicesButton.Click();
            Assert.AreEqual("CVEView - ReCVEServer", driver.Title);
        }

        [TestMethod]
        public void CveTableHasDataTest() {
            driver.Url = "https://localhost:7025/nist/CVEView";
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            wait.Until(drv => drv.FindElement(By.Id("cve_table")));
            IWebElement table = driver.FindElement(By.Id("cve_table"));
            var rows = table.FindElements(By.TagName("tr"));
            Assert.IsTrue(rows.Count > 1, "The table does not contain data.");
        }

        [TestMethod]
        public void DevicesAreFound() {
            driver.Url = "https://localhost:7025/Device";
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            wait.Until(drv => drv.FindElement(By.Id("clients_table")));
            IWebElement table = driver.FindElement(By.Id("clients_table"));
            var rows = table.FindElements(By.TagName("tr"));
            Assert.IsTrue(rows.Count > 1, "The table does not contain data.");
        }

        [TestMethod]
        public void UsageWorking() {
            driver.Url = "https://localhost:7025/Device/Usage";
            IWebElement usageButton = driver.FindElement(By.Id("getUsageBtn"));
            usageButton.Click();
            Thread.Sleep(3000);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            var javaScriptExecutor = (IJavaScriptExecutor)driver;
            IWebElement table = driver.FindElement(By.Id("processesTable"));
            var rows = table.FindElements(By.TagName("tr"));
            wait.Until(drv => drv.FindElement(By.Id("usageGraph")));
            var chartData = javaScriptExecutor.ExecuteScript("return Highcharts.charts[0].series[0].data.length;");
            Assert.IsTrue(Convert.ToInt32(chartData) > 0, "Pie chart does not contain data.");
            Assert.IsTrue(rows.Count > 1, "The table does not contain data.");

        }




        [TestMethod]
        public void TestPieChartHasData() {
            driver.Url = "https://localhost:7025/nist/CVEView";
            var javaScriptExecutor = (IJavaScriptExecutor)driver;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3)); //waits for the element to load
            wait.Until(drv => drv.FindElement(By.Id("cvePieChart")));
            var chartData = javaScriptExecutor.ExecuteScript("return Highcharts.charts[0].series[0].data.length;");
            Assert.IsTrue(Convert.ToInt32(chartData) > 0, "Pie chart does not contain data.");
        }

        [TestMethod]
        public void TestLineGraphHasData() {
            driver.Url = "https://localhost:7025/nist/CVEView";
            var javaScriptExecutor = (IJavaScriptExecutor)driver;
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            wait.Until(drv => drv.FindElement(By.Id("cveHistoryChart")));
            var chartData = javaScriptExecutor.ExecuteScript("return Highcharts.charts[1].series[0].data.length;");
            Assert.IsTrue(Convert.ToInt32(chartData) > 0, "Line graph does not contain data.");
        }

        [TestMethod]
        public void ClickDevicesButton() {
            driver.Url = "https://localhost:7025/"; 
            IWebElement devicesButton = driver.FindElement(By.Id("devicesBtn"));
            devicesButton.Click();
            Assert.AreEqual("Devices - ReCVEServer", driver.Title);
        }

        [TestMethod]
        public void ClickUsageButton() {
            driver.Url = "https://localhost:7025/";
            IWebElement usageButton = driver.FindElement(By.Id("usageBtn"));
            usageButton.Click();
            Assert.AreEqual("Usage - ReCVEServer", driver.Title);
        }

        [TestMethod]
        public void ClickRemoteButton() {
            driver.Url = "https://localhost:7025/";
            IWebElement remoteButton = driver.FindElement(By.Id("remoteBtn"));
            remoteButton.Click();
            Assert.AreEqual("Dashboard - ReCVEServer", driver.Title);
        }

        [TestMethod]
        public void ClickSettingsButton() {
            driver.Url = "https://localhost:7025/"; 
            IWebElement settingsButton = driver.FindElement(By.Id("settingsBtn"));
            settingsButton.Click();
            Assert.AreEqual("Dashboard - ReCVEServer", driver.Title);
        }

    }
    
}