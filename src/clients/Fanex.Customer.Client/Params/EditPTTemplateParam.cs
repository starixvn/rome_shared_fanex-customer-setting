using System.Collections.Generic;
using Fanex.Customer.Client.Models;

namespace Fanex.Customer.Client.Params
{
    public class EditPTTemplateParam : PTTemplateBaseParam
    {
        public EditPTTemplateParam(
            int custId,
            int templateId,
            bool isDefault,
            IEnumerable<PTTemplateSetting> settings,
            int subAccId,
            string language,
            string ip)
            : base(custId, subAccId, language, ip)
        {
            TemplateId = templateId;
            IsDefault = isDefault;
            Settings = settings;
        }

        public int TemplateId { get; }

        public bool IsDefault { get; }

        public IEnumerable<PTTemplateSetting> Settings { get; }
    }
}
