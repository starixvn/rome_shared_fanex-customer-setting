using System.ComponentModel;
using System.Configuration;
using Fanex.Customer.Client.Configurations;

namespace Fanex.Customer.Client.NETFramework
{
    public static class CustomerClientConfigurationExtension
    {
        public static CustomerClientConfiguration Initialize(this CustomerClientConfiguration configuration)
        {
            configuration = new CustomerClientConfiguration(
                GetValue<string>("Fanex.Customer:ServiceUri"),
                GetValue("Fanex.Customer:SystemId", 1),
                GetValue("Fanex.Customer:AccessDeniedUrl", "~/User/AccessDenied"));

            return configuration;
        }

        private static T GetValue<T>(string key, T defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            return string.IsNullOrEmpty(value) ? defaultValue : GetValue<T>(key);
        }

        private static T GetValue<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}
