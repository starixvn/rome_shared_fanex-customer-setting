namespace Alpha.CloneAccount.Api.Tests.Controllers
{
    using System.Threading.Tasks;
    using Alpha.CloneAccount.Constants;
    using Api.Controllers;
    using Api.Requests;
    using MediatR;
    using NSubstitute;
    using Xunit;

    [Trait("Category", "Controllers")]
    public class CustomerControllerTests
    {
        // private readonly IMediator mediator;
        // private readonly CustomerController customerController;

        // public CustomerControllerTests()
        // {
        //     mediator = Substitute.For<IMediator>();
        //     customerController = new CustomerController(mediator);
        // }

        // [Fact]
        // public async Task ValidateMaxNewMember_ValidParams_CallsMediatorSendValidateMaxNewMemberQuery()
        // {
        //     // Arrange
        //     var custId = 33120484;
        //     var language = "en-us";

        //     // Act
        //     await customerController.ValidateMaxNewMember(custId, language);

        //     // Assert
        //     await mediator
        //         .Received()
        //         .Send(Arg.Is<ValidateMaxNewMemberQuery>(
        //             q => q.AgentId == custId && q.Language == language));
        // }

        // [Fact]
        // public async Task ValidateMaxNewMember_InvalidParams_NotCallMediatorSendValidateMaxNewMemberQuery()
        // {
        //     // Arrange
        //     var custId = 0;
        //     var language = "en-us";

        //     // Act
        //     await customerController.ValidateMaxNewMember(custId, language);

        //     // Assert
        //     await mediator
        //         .DidNotReceive()
        //         .Send(Arg.Is<ValidateMaxNewMemberQuery>(
        //             q => q.AgentId == custId && q.Language == language));
        // }

        // [Fact]
        // public async Task GetPT_ValidParams_CallsMediatorSendCustomerPTInfoQuery()
        // {
        //     // Arrange
        //     var custId = 33120484;
        //     byte custLevel = 4;

        //     // Act
        //     await customerController.GetPT(custId, custLevel);

        //     // Assert
        //     await mediator
        //         .Received()
        //         .Send(Arg.Is<CustomerPTInfoQuery>(
        //             q => q.CustId == custId && q.CustLevel == custLevel));
        // }

        // [Fact]
        // public async Task GetPT_InvalidCustId_NotCallMediatorSendCustomerPTInfoQuery()
        // {
        //     // Arrange
        //     var custId = 0;
        //     byte custLevel = 4;

        //     // Act
        //     await customerController.GetPT(custId, custLevel);

        //     // Assert
        //     await mediator
        //         .DidNotReceive()
        //         .Send(Arg.Is<CustomerPTInfoQuery>(
        //             q => q.CustId == custId && q.CustLevel == custLevel));
        // }

        // [Fact]
        // public async Task GetPT_InvalidCustLevel_NotCallMediatorSendCustomerPTInfoQuery()
        // {
        //     // Arrange
        //     var custId = 33120484;
        //     byte custLevel = 10;

        //     // Act
        //     await customerController.GetPT(custId, custLevel);

        //     // Assert
        //     await mediator
        //         .DidNotReceive()
        //         .Send(Arg.Is<CustomerPTInfoQuery>(
        //             q => q.CustId == custId && q.CustLevel == custLevel));
        // }

        // [Fact]
        // public async Task Copy_ValidParams_CallsMediatorSendCopyCustomerCommand()
        // {
        //     // Arrange
        //     var command = new CopyCustomerCommand
        //     {
        //         ActorId = 8,
        //         ActorUserLevel = 4,
        //         UserName = "master",
        //         Credit = 1000000,
        //         MaxCredit = 0,
        //         SourceCustId = 10,
        //         Language = "en-US",
        //         SystemId = SystemIdentity.Chatbot
        //     };

        //     // Act
        //     await customerController.Copy(command);

        //     // Assert
        //     await mediator
        //         .Received()
        //         .Send(Arg.Is<CopyCustomerCommand>(
        //             c => c.ActorId == command.ActorId
        //             && c.ActorUserLevel == command.ActorUserLevel
        //             && c.UserName == command.UserName
        //             && c.Credit == command.Credit
        //             && c.MaxCredit == command.MaxCredit
        //             && c.SourceCustId == command.SourceCustId
        //             && c.Language == command.Language
        //             && c.SystemId == command.SystemId));
        // }

        // [Fact]
        // public async Task Copy_InvalidParams_NotCallMediatorSendCopyCustomerCommand()
        // {
        //     // Arrange
        //     var command = new CopyCustomerCommand
        //     {
        //         ActorUserLevel = 4,
        //         Language = "en-US"
        //     };

        //     // Act
        //     await customerController.Copy(command);

        //     // Assert
        //     await mediator
        //         .DidNotReceive()
        //         .Send(Arg.Any<CopyCustomerCommand>());
        // }

        // [Fact]
        // public async Task CheckReachLimitDownline_Always_CallsMediatorSendCheckReachLimitDownlineQuery()
        // {
        //     // Arrange
        //     var query = new CheckReachLimitDownlineQuery
        //     {
        //         CustId = 1,
        //         CustLevel = 2,
        //         Username = "username",
        //         Language = "en-US"
        //     };

        //     // Act
        //     await customerController.CheckReachLimitDownline(query);

        //     // Assert
        //     await mediator.Received().Send(Arg.Is<CheckReachLimitDownlineQuery>(
        //         q => q.CustId == query.CustId
        //             && q.CustLevel == query.CustLevel
        //             && query.Username == q.Username
        //             && q.Language == query.Language));
        // }

        // [Fact]
        // public async Task CheckLimitNewDownline_Always_CallsMediatorSendCheckLimitNewDownlineQuery()
        // {
        //     // Arrange
        //     var query = new CheckLimitNewDownlineQuery
        //     {
        //         CustId = 1,
        //         CustLevel = 2,
        //         Language = "en-US"
        //     };

        //     // Act
        //     await customerController.CheckLimitNewDownline(query);

        //     // Assert
        //     await mediator.Received().Send(Arg.Is<CheckLimitNewDownlineQuery>(
        //         q => q.CustId == query.CustId
        //             && q.CustLevel == query.CustLevel
        //             && q.Language == query.Language));
        // }
    }
}