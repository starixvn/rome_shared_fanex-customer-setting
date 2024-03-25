namespace Fanex.Customer.API.Modules.EditCustomer.Models;

internal class UnderMaintainanceInfo
{
    public UnderMaintainanceInfo() : this(0, 0, 0, null, null) { }

    public UnderMaintainanceInfo(byte productId, short settingType, short sportType, DateTime? beginTime, DateTime? endTime)
    {
        this.ProductId = productId;
        this.SettingType = settingType;
        this.SportType = sportType;
        this.BeginTime = beginTime;
        this.EndTime = endTime;
    }

    public byte ProductId { get; set; }

    public short SettingType { get; set; }

    public short SportType { get; set; }

    public DateTime? BeginTime { get; set; }

    public DateTime? EndTime { get; set; }
}