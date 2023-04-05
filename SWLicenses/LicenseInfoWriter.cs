namespace SWLicenses
{
    internal class LicenseInfoWriter : IDisposable
    {
        private readonly StreamWriter _sw;

        internal LicenseInfoWriter(string filePath)
        {
            _sw = new StreamWriter(filePath);
            _sw.WriteLine("\"Product Name\",\"Serial Number\",\"Activated Computer\",\"Maintenance End Date\"");
        }

        internal void Write(LicenseInfo licenseInfo)
        {
            _sw.WriteLine(licenseInfo.ToString());
        }

        public void Dispose()
        {
            _sw?.Dispose();
        }
    }
}