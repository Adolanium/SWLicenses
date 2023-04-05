namespace SWLicenses
{
    internal static class Configuration
    {
        internal static async Task<string[]> LoadSerialsAsync()
        {
            string[] serials = await File.ReadAllLinesAsync("serials.txt");
            return serials.Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToArray();
        }

        internal static async Task<string[]> LoadCredentialsAsync()
        {
            return await File.ReadAllLinesAsync("credentials.txt");
        }
    }
}