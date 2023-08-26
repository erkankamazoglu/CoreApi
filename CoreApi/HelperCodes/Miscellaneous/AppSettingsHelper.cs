using System.IO;
using Microsoft.Extensions.Configuration;

namespace CoreApi.HelperCodes.Miscellaneous
{
    public static class AppSettingsHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            IConfigurationRoot root = configurationBuilder.Build();
            return root;
        } 

        public static string GetConnectionString()
        {
            return GetIConfigurationRoot().GetSection("ConnectionStrings:cs").Value;
        }
    }
}