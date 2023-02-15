namespace Alpha.CloneAccount.Models.Keno;

public class PositionTakingBase
{
    public int SportTypeId { get; set; }

    public int BetTypeId { get; set; }

    public string BetTypeName { get; set; }

    public double S4 { get; set; }

    public double M4 { get; set; }

    public double A4 { get; set; }

    public double N4 { get; set; }

    public double L4 { get; set; }

    public double SComm { get; set; }

    public double SCommMax { get; set; }

    public double S3 { get; set; }

    public double M3 { get; set; }

    public double US3 { get; set; }

    public bool IS3 { get; set; }

    public double N3 { get; set; }

    public double L3 { get; set; }

    public double MComm { get; set; }

    public double M2 { get; set; }

    public double A2 { get; set; }

    public double UM2 { get; set; }

    public bool IM2 { get; set; }

    public double N2 { get; set; }

    public double L2 { get; set; }

    public double AComm { get; set; }

    public double A1 { get; set; }

    public double Comm { get; set; }

    public double S1 { get; set; }

    public double US1 { get; set; }

    public bool IS1 { get; set; }

    public double M1 { get; set; }

    public double UM1 { get; set; }

    public bool IM1 { get; set; }

    public void AdjustValues()
    {
        AdjustMaster();
        AdjustAgent();
        AdjustSuperOnMember();
        AdjustMasterOnMember();
    }

    internal void AdjustAgent()
    {
        if (IM2)
        {
            M2 = 0;
            return;
        }

        UM2 = 0;
    }

    internal void AdjustMaster()
    {
        if (IS3)
        {
            S3 = 0;
            return;
        }

        US3 = 0;
    }

    internal void AdjustSuperOnMember()
    {
        if (IS1)
        {
            S1 = 0;
            return;
        }

        US1 = 0;
    }

    internal void AdjustMasterOnMember()
    {
        if (IM1)
        {
            M1 = 0;
            return;
        }

        UM1 = 0;
    }
}