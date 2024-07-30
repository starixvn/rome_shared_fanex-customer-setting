using Fanex.Data.Repository;
namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetGetBackupDomainsCriteria: CriteriaBase
{
    public GetGetBackupDomainsCriteria(string site)
        => Site = site;

    public string Site { get; }

    public override string GetSettingKey()
        => "Age_DomainListSel";

    public override bool IsValid() => true;
}