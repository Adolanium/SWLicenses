namespace SWLicenses
{
    internal class LicenseInfo
    {
        private string ProductName { get; set; }
        private string Version { get; set; }
        private string SerialNumber { get; set; }
        private string ActivatedComputer { get; set; }
        private DateTime MaintenanceEndDate { get; set; }

        internal LicenseInfo(string productName, string serialNumber, string maintenanceEndDate, string version)
        {
            ProductName = productName;
            Version = version;
            SerialNumber = serialNumber;
            ActivatedComputer = "N/A";
            MaintenanceEndDate = DateTime.Parse(maintenanceEndDate);
        }

        public LicenseInfo(string productName, string serialNumber, string activatedComputer, DateTime maintenanceEndDate, string version)
        {
            ProductName = productName;
            Version = version;
            SerialNumber = serialNumber;
            ActivatedComputer = activatedComputer;
            MaintenanceEndDate = maintenanceEndDate;
        }

        internal void Write(StreamWriter sw)
        {
            sw.WriteLine(ToString());
        }

        internal void SetActivatedComputer(string activatedComputer)
        {
            ActivatedComputer = activatedComputer;
        }

        internal void SetVersion(string version)
        {
            Version = version;
        }

        public override string ToString()
        {
            return $"\"{ProductName}\",\"{Version}\",\"{SerialNumber}\",\"{ActivatedComputer}\",\"{MaintenanceEndDate.ToString("yyyy-MM-dd")}\"";
        }
    }
}