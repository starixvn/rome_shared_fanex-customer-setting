namespace Fanex.Customer.API.Modules.Message;

public static class MessageTypes
{
    public const int SimpleMessage = 0;

    public const int ParlayMaxPayoutViolation = 31;

    public const int WinLossLimitViolation = 41;

    public const int CommissionViolation = 51;

    public const int SBCommissionViolation = 52;

    public const int SBMemberCommissionViolation = 53;

    public const int DownlinePTViolation = 1;

    public const int MinPTViolation = 2;

    public const int WrongAutoPT = 60;

    public const int WrongPT = 61;

    public const int WrongPT_Edit = 5;

    public const int MaxPTViolation = 63;

    public const int MaxPTViolation_Edit = 3;

    public const int MinMaxPTViolation = 62;

    public const int MinMaxPTViolation_Edit = 4;

    public const int BetSettingViolation = 71;

    public const int UpdateTooOften = 6;

    public const int ProductStatusViolation = 91;

    public const int ProductStatusDependencyViolation = 92;

    public const int StakeLevelViolation = 131;

    public const int WinLimitViolatedWithUpline = 111;

    public const int LossLimitViolatedWithUpline = 112;
    
    public const int DailyAutoResetViolatedWithUpline = 113;
}