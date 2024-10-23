using System.Text.Json.Serialization;

namespace Licensee.AdminSetting.API.Features.Shared
{
    public class Result(int errorCode, string errorMessage)
    {
        public int ErrorCode { get; } = errorCode;

        public string ErrorMessage { get; } = errorMessage;

        [JsonIgnore]
        public bool IsOK => ErrorCode == 0;

        [JsonIgnore]
        public bool IsNotOK => !IsOK;
    }
}