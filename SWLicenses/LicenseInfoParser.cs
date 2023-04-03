using System.Collections.Generic;
using OpenQA.Selenium;
using SWLicenses;

namespace SWLicenses
{
    internal class LicenseInfoParser
    {
        public static LicenseInfo ParseLicenseInfo(IWebDriver driver)
        {
            var productName = driver.FindElement(By.Id("lblProdName")).Text;
            var serialNumber = driver.FindElement(By.Id("lblSerialNumber")).Text;
            var maintEnd = driver.FindElement(By.Id("lblMaintEnd")).Text;

            return new LicenseInfo(productName, serialNumber, maintEnd);
        }

        public static List<IWebElement> GetReportTableRows(IWebDriver driver)
        {
            return driver.FindElements(By.XPath(@"//*[@id=""dgReport""]/tbody/tr")).ToList();
        }
    }
}