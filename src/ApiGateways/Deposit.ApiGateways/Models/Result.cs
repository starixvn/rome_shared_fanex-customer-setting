namespace Deposit.ApiGateways.Models
{
    public class Result(int code, string type, string message)
    {
        public int Code { get; } = code;

        public string Type { get; } = type;

        public bool IsOk => Code == 0;

        public bool IsNotOk => !IsOk;

        public string Message { get; } = message;

        public static Result CreateSuccessfulResult(string type = "", string message = "") => new(0, type, message);

        public static Result CreateErrorResult(string type = "", string message = "") => new(1, type, message);
    }
}