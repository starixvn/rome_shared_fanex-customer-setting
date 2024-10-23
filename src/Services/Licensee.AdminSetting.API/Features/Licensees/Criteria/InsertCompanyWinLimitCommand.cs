using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class InsertCompanyWinLimitCommand(int licenseeId) : NonQueryCommand
    {
        public int LicenseeId { get; } = licenseeId;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "Dot_Dep_CWL_InsertSetting";

        public override bool IsValid() => LicenseeId > 0;
    }
}