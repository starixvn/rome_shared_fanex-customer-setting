using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Accounts.Criteria
{
    public class RollbackCommand(int custId) : NonQueryCommand
    {
        public int CustId { get; } = custId;

        public override string GetSettingKey() => "Acc_CustomerIns_Rollback";

        public override bool IsValid() => CustId > 0;
    }
}