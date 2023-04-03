using System;
using System.IO;
using SWLicenses;

namespace SWLicenses
{
    internal class LicenseInfoWriter : IDisposable
    {
        private readonly StreamWriter _sw;

        public LicenseInfoWriter(string filePath)
        {
            _sw = new StreamWriter(filePath);
            _sw.WriteLine("\"Product Name\",\"Serial Number\",\"Activated Computer\",\"Maintenance End Date\"");
        }

        public void Write(LicenseInfo licenseInfo)
        {
            _sw.WriteLine(licenseInfo.ToString());
        }

        public void Dispose()
        {
            _sw?.Dispose();
        }
    }
}