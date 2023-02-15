namespace Alpha.CloneAccount.Configurations;

public class AppSettings
{
    public const string Setting = "Settings";

    public byte SettingStatus { get; set; }

    public static int RetryTimesIfUserNameNotAvailable { get; set; }
}