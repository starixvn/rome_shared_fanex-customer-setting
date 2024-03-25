
namespace Fanex.Customer.Client.Params
{
    public class DeletePTTemplateParam : PTTemplateBaseParam
    {
        public DeletePTTemplateParam(
            int templateId,
            int custId,
            int subAccId,
            string language,
            string ip)
            : base(custId, subAccId, language, ip)
        {
            TemplateId = templateId;
        }

        public int TemplateId { get; }
    }
}
