namespace ONEbook.UM.ThirdParty.AspNetCore.Configurations;

public class UMClientConfiguration
{
    public const string ThirdPartyUM = "ONEbook.UM.ThirdParty";

    public string ServiceUris { get; set; }

    public bool EnableCheckUM { get; set; } = false;

    public bool EnableUnscheduledUM { get; set; } = false;

    public string UMFrom { get; set; }

    public string UMTo { get; set; }


    public int SiteId { get; set; } = 4120;

    public int ResultCacheDuration { get; set; } = 60;

    public ProductUMConfiguration[] Products { get; set; }
}

public class ProductUMConfiguration
{
    public int Id { get; set; }

    public bool IsUM { get; set; }

    public string UMFrom { get; set; }

    public string UMTo { get; set; }
}