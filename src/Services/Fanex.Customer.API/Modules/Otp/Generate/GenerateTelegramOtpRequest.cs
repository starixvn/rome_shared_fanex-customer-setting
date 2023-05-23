namespace Fanex.Customer.API.Modules.Otp;

public class GenerateTelegramOtpRequest
{
    public int CustId { get; set; }

    public int SubAccId { get; set; }

    public bool IsValid()
        => CustId > 0 && SubAccId >= 0;
}