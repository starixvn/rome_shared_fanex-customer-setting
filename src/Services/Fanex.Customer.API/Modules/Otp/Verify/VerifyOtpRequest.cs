using System.ComponentModel.DataAnnotations;

namespace Fanex.Customer.API.Modules.Otp;

public class VerifyOtpRequest
{
    [Required]
    public int CustId { get; set; }

    public int SubAccId { get; set; }

    [Required]
    public string Otp { get; set; }

    public string IP { get; set; }

    public string Language { get; set; }

    public bool IsEnabledTelegramOTP { get; set; }
}