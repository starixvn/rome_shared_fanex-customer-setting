using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class UpdateCustomerInfoCommand : NonQueryCommand
{
    public UpdateCustomerInfoCommand(
        int custId,
        string firstName,
        string lastName,
        string phone,
        string mobilePhone,
        string email,
        string fax,
        int userId,
        int subAccId,
        string ip,
        string language,
        string remark)
    {
        CustId = custId;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        MobilePhone = mobilePhone;
        Email = email;
        Fax = fax;
        UserId = userId;
        SubAccId = subAccId;
        IP = ip;
        Language = language;
        Remark = remark;
    }

    public int CustId { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string Phone { get; }

    public string MobilePhone { get; }

    public string Email { get; }

    public string Fax { get; }

    public int UserId { get; }

    public int SubAccId { get; }

    public string IP { get; }

    public string Language { get; }

    public string Remark { get; }

    public int ErrorCode { get; set; }

    public string ErrorMsg { get; set; }

    public override string GetSettingKey() => "Age_CustomerUpdUserInfo";

    public override bool IsValid() => CustId > 0 && UserId > 0 && SubAccId >= 0;
}