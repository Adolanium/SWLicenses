using System.IO;
using System.Threading.Tasks;

namespace SWLicenses
{
    public static class Configuration
    {
        public static async Task<string[]> LoadSerialsAsync()
        {
            string[] serials = await File.ReadAllLinesAsync("serials.txt");
            return serials.Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToArray();
        }

        public static async Task<string[]> LoadCredentialsAsync()
        {
            return await File.ReadAllLinesAsync("credentials.txt");
        }
    }
}