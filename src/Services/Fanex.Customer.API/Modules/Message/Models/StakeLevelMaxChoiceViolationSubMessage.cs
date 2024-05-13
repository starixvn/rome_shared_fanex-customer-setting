using System.Xml.Serialization;

namespace Fanex.Customer.API.Modules.Message.Models;

public class StakeLevelMaxChoiceViolationSubMessage : SubMessage
{
    [XmlAttribute]
    public string MaxChoice { get; set; }
}