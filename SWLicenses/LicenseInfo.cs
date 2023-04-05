namespace SWLicenses
{
    internal class LicenseInfo
    {
        private string ProductName { get; set; }
        private string SerialNumber { get; set; }
        private string ActivatedComputer { get; set; }
        private DateTime MaintenanceEndDate { get; set; }

        internal LicenseInfo(string productName, string serialNumber, string maintenanceEndDate)
        {
            ProductName = productName;
            SerialNumber = serialNumber;
            ActivatedComputer = "N/A";
            MaintenanceEndDate = DateTime.Parse(maintenanceEndDate);
        }

        public LicenseInfo(string productName, string serialNumber, string activatedComputer, DateTime maintenanceEndDate)
        {
            ProductName = productName;
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


        public override string ToString()
        {
            return $"\"{ProductName}\",\"{SerialNumber}\",\"{ActivatedComputer}\",\"{MaintenanceEndDate.ToString("yyyy-MM-dd")}\"";
        }
    }
}