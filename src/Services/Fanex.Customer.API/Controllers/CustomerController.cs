using Alpha.CloneAccount.Api.Requests;
using Alpha.CloneAccount.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.CloneAccount.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly IMediator mediator;

    public CustomerController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("{agentId}/ValidateMaxNewMember")]
    public async Task<Result> ValidateMaxNewMember(int agentId, string language)
    {
        var message = new ValidateMaxNewMemberQuery(agentId, language);

        return message.IsValid()
          ? await mediator.Send(message)
          : new Result(400, "Bad Request.");
    }

    [HttpGet("{custId}/{custLevel}/pt")]
    public async Task<CustomerPTInfo> GetPT(int custId, byte custLevel)
    {
        var message = new CustomerPTInfoQuery(custId, custLevel);

        return message.IsValid()
          ? await mediator.Send(message)
          : new CustomerPTInfo { CustId = custId };
    }

    [HttpPost("copy")]
    public async Task<CopyCustomerResult> Copy([FromBody] CopyCustomerCommand command)
        => command?.IsValid() == true
        ? await mediator.Send(command)
        : new CopyCustomerResult(0, null, null, 0, null, 0, 0, 400, "Bad Request.");

    [HttpPost("CheckReachLimitDownline")]
    public async Task<Result> CheckReachLimitDownline([FromBody] CheckReachLimitDownlineQuery request)
        => await mediator.Send(request);

    [HttpPost("CheckLimitNewDownline")]
    public async Task<Result> CheckLimitNewDownline([FromBody] CheckLimitNewDownlineQuery request)
        => await mediator.Send(request);
}