﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SWLicenses
{
    internal class SwActivationManagerDriver
    {
        private const string Url = "https://activate.solidworks.com/manager/";
        private static readonly string[] Credentials = File.ReadAllLines("credentials.txt");

        public static IWebDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");

            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            var driver = new ChromeDriver(service, options);
            driver.Navigate().GoToUrl(Url);
            return driver;
        }

        public static void Login(IWebDriver driver)
        {
            driver.FindElement(By.Id("Login2_txtName")).SendKeys(Credentials[0]);
            driver.FindElement(By.Id("Login2_txtPassword")).SendKeys(Credentials[1]);
            driver.FindElement(By.Id("Login2_cmdLogin")).Click();
        }

        public static void LookupSerial(IWebDriver driver, string serial)
        {
            driver.FindElement(By.Id("txtSearch")).SendKeys(serial);
            driver.FindElement(By.Id("cmdLookup")).Click();
            driver.FindElement(By.LinkText("View")).Click();
        }

        public static async Task LookupSerials(IWebDriver driver, string[] serials, IProgress<int> progress)
        {
            using var sw = new StreamWriter("results.csv");
            sw.WriteLine("\"Product Name\",\"Serial Number\",\"Activated Computer\",\"Maintenance End Date\"");

            int total = serials.Length;
            int completed = 0;

            Console.WriteLine("Processing serials:");
            foreach (string serial in serials)
            {
                try
                {
                    LookupSerial(driver, serial);

                    var reportTableRows = driver.FindElements(By.XPath(@"//*[@id=""dgReport""]/tbody/tr")).ToList();
                    int numRows = reportTableRows.Count;

                    var productName = driver.FindElement(By.Id("lblProdName")).Text;
                    var serialNumber = driver.FindElement(By.Id("lblSerialNumber")).Text;
                    var maintEnd = driver.FindElement(By.Id("lblMaintEnd")).Text;

                    var license = new License(productName, serialNumber, maintEnd);

                    bool hasActiveComputer = false;
                    foreach (var row in reportTableRows.Skip(2))
                    {
                        var rowData = row.FindElements(By.TagName("td")).ToList();
                        if (rowData[2].Text.Equals("Y"))
                        {
                            license.ActivatedComputer = rowData[1].Text;
                            license.Write(sw);
                            hasActiveComputer = true;
                        }
                    }

                    if (!hasActiveComputer)
                    {
                        license.Write(sw);
                    }

                    driver.FindElement(By.LinkText("Log Out")).Click();
                    Login(driver);
                }
                catch
                {
                    sw.WriteLine($"N/A,{serial},N/A,N/A");
                    driver.FindElement(By.LinkText("Log Out")).Click();
                    Login(driver);
                }

                completed++;
                int percent = completed * 100 / total;
                progress.Report(percent);

                Console.Write($"\r[{GenerateProgressBar(percent, 50)}] {percent}%");
            }

            driver.Close();
            driver.Quit();
        }

        private static string GenerateProgressBar(int progress, int width)
        {
            int completedWidth = progress * width / 100;
            return new string('#', completedWidth) + new string(' ', width - completedWidth);
        }
    }
}
