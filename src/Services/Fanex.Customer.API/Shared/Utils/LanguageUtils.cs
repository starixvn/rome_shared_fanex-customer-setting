namespace Fanex.Customer.API.Shared.Utils;

public static class LanguageUtils
{
    public const string DefaultLanguage = "en-US";

    public static string ToLanguageKey(string language)
    {
        var lowerLanguage = string.IsNullOrWhiteSpace(language)
            ? "en-us"
            : language.ToLowerInvariant();

        return lowerLanguage switch
        {
            "en-us" => "E",
            "ja-jp" => "J",
            "zh-tw" => "CT",// Traditional
            "zh-cn" => "CS",// Simplified,
            "zh-hant" => "CT",// Traditional
            "zh-hans" => "CS",// Simplified
            "th-th" => "TH",
            "ko-kr" => "KO",
            "vi-vn" => "VI",
            _ => "E",
        };
    }
}

