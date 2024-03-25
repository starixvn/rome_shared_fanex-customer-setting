using System.Collections.Generic;
using Fanex.Customer.Client.Models;

namespace Fanex.Customer.Client.Params
{
    public class CreatePTTemplateParam : PTTemplateBaseParam
    {
        public CreatePTTemplateParam(
            int custId,
            string templateName,
            bool isDefault,
            IEnumerable<PTTemplateSetting> settings,
            int subAccId,
            string language,
            string ip)
            : base(custId, subAccId, language, ip)
        {
            TemplateName = templateName;
            IsDefault = isDefault;
            Settings = settings;
        }

        public string TemplateName { get; }

        public bool IsDefault { get; }

        public IEnumerable<PTTemplateSetting> Settings { get; }
    }
}
