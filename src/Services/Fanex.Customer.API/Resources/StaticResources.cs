namespace Fanex.Customer.API.Resources
{
    using System.Globalization;

    public static class StaticResources
    {
        public static string Get(string key, string language)
            => Resources.ResourceManager.GetString(key, CultureInfo.CreateSpecificCulture(language));

        public static string Get(string key, CultureInfo culture)
            => Resources.ResourceManager.GetString(key, culture);
    }
}
