using System.Xml.Serialization;

namespace Fanex.Customer.API.Modules.Message.Models;

[XmlRoot("Root")]
public class MainMessage<T> : RootMessage where T : SubMessage
{
    [XmlElement("Row")]
    public T[] SubMessages { get; set; }
}