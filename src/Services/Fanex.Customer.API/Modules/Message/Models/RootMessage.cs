using System.Xml.Serialization;

namespace Fanex.Customer.API.Modules.Message.Models;

[XmlRoot("Root")]
public class RootMessage
{
    [XmlAttribute("MsgID")]
    public int MessageId { get; set; }

    [XmlAttribute("Msg")]
    public string Message { get; set; }
}