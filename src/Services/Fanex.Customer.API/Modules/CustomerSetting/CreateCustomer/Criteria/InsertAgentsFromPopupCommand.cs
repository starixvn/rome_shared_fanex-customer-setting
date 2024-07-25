using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class InsertSuperToAutoTransferCommand : INonQueryCommand
{
    public InsertSuperToAutoTransferCommand(string userName, int transferType, int userId, string ip)
    {
        UserName = userName;
        TransferType = transferType;
        UserID = userId;
        IP = ip;
    }

    public string UserName { get; }

    public int TransferType { get; }

    public int UserID { get; }

    public string IP { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public string GetSettingKey() => "Acc_CustomerInsAutoTransferAgent";

    public bool IsValid() => true;
}
