using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class UpdatePositionTakingTemplateResult : Result
{
    public UpdatePositionTakingTemplateResult(int code, int type, string message)
    : base(code, message)
    {
        Type = type;
    }

    public int Type { get; }
}