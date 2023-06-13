using System.Xml.Serialization;
using Fanex.Customer.API.Modules.Message.Utils;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class WrongPTSubMessage : SubMessage
{
    [XmlAttribute]
    public bool IsLive { get; set; }

    public override string ToString(ICacheRefService cacheRefService, byte settingLevel, string language)
    {
        var message = base.ToString(cacheRefService, settingLevel, language);
        if (IsLive)
        {
            message += string.Format(" ({0})", StaticResources.Get("Live", language));
        }

        return message;
    }
}