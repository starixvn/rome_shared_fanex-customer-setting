using System.Text;

namespace Licensee.AdminSetting.API.Features.Shared
{
    public static class ILoggerExtensions
    {
        public static Task LogErrExtraInfo(this Fanex.Logging.ILogger logger, string errExtraInfo, string settingKey)
        {
            var message = new StringBuilder();
            message.AppendLine($"ErrExtraInfo: {errExtraInfo}");
            message.AppendLine($"SettingKey: {settingKey}");
            return logger.ErrorAsync(message.ToString());
        }
    }
}