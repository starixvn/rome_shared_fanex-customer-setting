using System.Xml.Serialization;

namespace Fanex.Customer.API.Modules.Message.Models;

public class ProductStatusViolationSubMessage : SubMessage
{
    [XmlAttribute]
    public short ParentSportType { get; set; }

    [XmlAttribute]
    public short ParentBetType { get; set; }
}
