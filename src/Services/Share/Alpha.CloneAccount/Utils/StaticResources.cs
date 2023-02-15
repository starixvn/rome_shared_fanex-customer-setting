namespace Alpha.CloneAccount.Utils
{
    using System.Globalization;
    using Alpha.CloneAccount.AppResources;

    public static class StaticResources
    {
        public static string Get(string key, string language)
            => Resources.ResourceManager.GetString(key, CultureInfo.CreateSpecificCulture(language));

        public static string Get(string key, CultureInfo culture)
            => Resources.ResourceManager.GetString(key, culture);
    }
}
