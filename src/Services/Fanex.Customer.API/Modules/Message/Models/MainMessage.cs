using System.Xml.Serialization;

namespace Fanex.Customer.API.Modules.Message.Models;

public class MainMessage<T> : RootMessage where T : SubMessage
{
    [XmlElement("Row")]
    public T[] SubMessages { get; set; }
}