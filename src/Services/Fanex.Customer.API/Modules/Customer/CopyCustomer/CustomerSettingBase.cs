using Fanex.Logging;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class CustomerSettingBase<T> where T : PositionTakingBase
{
    public CustomerInformation CustomerInformation { get; set; }

    public IList<BetSetting> BetSettings { get; set; }

    public IList<T> PositionTakings { get; set; }

    public CopyAll BetSettingCopyAll { get; set; }

    public CopyAll StatusCopyAll { get; set; }

    public CopyAll PositionTakingCopyAll { get; set; }

    public void AdjustInformationWhenUpdateSetting()
    {
        CopyBetSettingInCaseOfCopyAll();
        CopyStatusInCaseOfCopyAll();
        CopyPositionTakingInCaseOfCopyAll();
        AdjustValuesOfPositionTakings();
        UpdateBetSettingWhenMultiple();
        UpdatePositionTakingWhenMultiple();
    }

    public void AdjustInformationWhenLoadSetting(int custId, int userId, int subAccountId)
    {
        try
        {
            UpdateCustomerInformation(custId, userId, subAccountId);
            UpdateCopyAllInformation();
        }
        catch (Exception ex)
        {
            Logger.Log.Error(ex.Message, ex);
        }
    }

    public void UpdateBetSettingWhenMultiple()
    {
        if (CustomerInformation.IsMultiple)
        {
            foreach (var betSetting in BetSettings)
            {
                betSetting.IsBetSettingUpdated = true;
                betSetting.IsStatusUpdated = true;
            }
        }
    }

    public void UpdatePositionTakingWhenMultiple()
    {
        if (CustomerInformation.IsMultiple)
        {
            foreach (var betSetting in BetSettings)
            {
                betSetting.IsBetSettingUpdated = true;
                betSetting.IsStatusUpdated = true;
            }
        }

        foreach (var positionTaking in PositionTakings)
        {
            ChangeSettingWhenMultiple(positionTaking, CustomerInformation.IsMultiple);
        }
    }

    internal static bool IsCopyToItself(int currentBetTypeId, int copiedBetTypeId)
    {
        return currentBetTypeId == copiedBetTypeId;
    }

    internal static void CopyToOneBetSetting(BetSetting sourceBetSetting, BetSetting destinationBetSetting)
    {
        destinationBetSetting.MinBet = sourceBetSetting.MinBet;
        destinationBetSetting.MaxBet = sourceBetSetting.MaxBet;
        destinationBetSetting.MaxPerMatch = sourceBetSetting.MaxPerMatch;
        destinationBetSetting.IsBetSettingUpdated = true;
    }

    internal static void CopyToOneStatus(BetSetting sourceBetSetting, BetSetting destinationBetSetting)
    {
        destinationBetSetting.IsEnabled = sourceBetSetting.IsEnabled;
        destinationBetSetting.IsStatusUpdated = true;
    }

    internal void UpdateCopyAllInformation()
    {
        PositionTakingCopyAll = new CopyAll();
        BetSettingCopyAll = new CopyAll();
        StatusCopyAll = new CopyAll();
    }

    internal void UpdateCustomerInformation(int custId, int userId, int subAccountId)
    {
        CustomerInformation.CustId = custId;
        CustomerInformation.UserId = userId;
        CustomerInformation.SubAccId = subAccountId;
    }

    /// <summary>
    /// Updates the position takings in case of copy all.
    /// </summary>
    internal void CopyPositionTakingInCaseOfCopyAll()
    {
        if (!CopyAllIsValidForPositionTaking())
        {
            return;
        }

        var copiedPositionTaking = GetCopiedPositionTaking();
        if (copiedPositionTaking.BetTypeId <= 0)
        {
            return;
        }

        foreach (var positionTaking in PositionTakings)
        {
            if (IsCopyToItself(positionTaking.BetTypeId, PositionTakingCopyAll.BetTypeId))
            {
                continue;
            }

            CopyToOnePositionTaking(copiedPositionTaking, positionTaking);
        }
    }

    internal void AdjustValuesOfPositionTakings()
    {
        foreach (var positionTaking in PositionTakings)
        {
            positionTaking.AdjustValues();
        }
    }

    internal bool CopyAllIsValidForPositionTaking()
    {
        return PositionTakingCopyAll != null && PositionTakingCopyAll.IsCopiedAll && PositionTakings != null && PositionTakings.Count > 0;
    }

    /// <summary>
    /// Updates the bet settings in case of copy all.
    /// </summary>
    internal void CopyBetSettingInCaseOfCopyAll()
    {
        if (CopyAllIsValidForBetSetting())
        {
            return;
        }

        var copiedBetSetting = GetCopiedBetSetting();
        if (copiedBetSetting.BetTypeId <= 0)
        {
            return;
        }

        foreach (var betSetting in BetSettings)
        {
            if (IsCopyToItself(betSetting.BetTypeId, BetSettingCopyAll.BetTypeId))
            {
                continue;
            }

            CopyToOneBetSetting(copiedBetSetting, betSetting);
        }
    }

    /// <summary>
    /// Updates the status in case of copy all.
    /// </summary>
    internal void CopyStatusInCaseOfCopyAll()
    {
        if (!CopyAllIsValidForStatus())
        {
            return;
        }

        var copiedBetSetting = GetCopiedStatus();
        if (copiedBetSetting.BetTypeId <= 0)
        {
            return;
        }

        foreach (var betSetting in BetSettings)
        {
            if (IsCopyToItself(betSetting.BetTypeId, BetSettingCopyAll.BetTypeId))
            {
                continue;
            }

            CopyToOneStatus(copiedBetSetting, betSetting);
        }
    }

    internal BetSetting GetCopiedBetSetting()
    {
        var betSetting = BetSettings.FirstOrDefault(b => b.BetTypeId == BetSettingCopyAll.BetTypeId);
        return betSetting ?? new BetSetting();
    }

    internal BetSetting GetCopiedStatus()
    {
        var betSetting = BetSettings.FirstOrDefault(b => b.BetTypeId == StatusCopyAll.BetTypeId);
        return betSetting ?? new BetSetting();
    }

    internal bool CopyAllIsValidForBetSetting()
    {
        return BetSettingCopyAll != null && BetSettingCopyAll.IsCopiedAll && BetSettings != null && BetSettings.Count > 0;
    }

    internal bool CopyAllIsValidForStatus()
    {
        return StatusCopyAll != null && StatusCopyAll.IsCopiedAll && BetSettings != null && BetSettings.Count > 0;
    }

    internal dynamic GetCopiedPositionTaking()
    {
        var positionTaking = PositionTakings.FirstOrDefault(p => p.BetTypeId == PositionTakingCopyAll.BetTypeId);
        return positionTaking ?? new PositionTakingBase();
    }

    internal virtual void ChangeSettingWhenMultiple(T positionTaking, bool isMultiple)
    {
    }

    internal virtual void CopyToOnePositionTaking(T sourcePositionTaking, T destinationPositionTaking)
    {
    }
}