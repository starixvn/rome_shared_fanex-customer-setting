using Fanex.Customer.API.Modules.Otp.Params;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Otp
{
    public class CustomerOtpEndpoint :
        IEndpoint<Result, ChangeOtpStatusParam, ICustomerOtpService>,
        IEndpoint<CustomerOtp, CustomerOtpParam, ICustomerOtpService>,
        IEndpoint<bool, CustomerOtpParamBase, ICustomerOtpService>,
        IEndpoint<Result, CustomerOtp, ICustomerOtpService>,
        IEndpoint<bool, CustomerOtpDevice, ICustomerOtpService>,
        IEndpoint<Result, RemoveSecretKeyParam, ICustomerOtpService>,
        IEndpoint<CustomerOtp, int, int, ICustomerOtpService>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            var otp = app.MapGroup("/api/customer/otp")
                .WithTags(EndpointTags.OTP);

            otp.MapPost("/change-status", (ChangeOtpStatusParam request1, ICustomerOtpService request2) => HandleAsync(request1, request2))
                .Produces<Result>();

            otp.MapPost("/generate", (CustomerOtpParam request1, ICustomerOtpService request2) => HandleAsync(request1, request2))
                .Produces<CustomerOtp>();

            otp.MapPost("/isenabled", (CustomerOtpParamBase request1, ICustomerOtpService request2) => HandleAsync(request1, request2))
                .Produces<bool>();

            otp.MapPost("/setup", (CustomerOtp request1, ICustomerOtpService request2) => HandleAsync(request1, request2))
                .Produces<Result>();

            otp.MapPost("/isdifferent-device", (CustomerOtpDevice request1, ICustomerOtpService request2) => HandleAsync(request1, request2))
                .Produces<bool>();

            otp.MapPost("/remove", (RemoveSecretKeyParam request1, ICustomerOtpService request2) => HandleAsync(request1, request2))
                .Produces<Result>();
            
            otp.MapGet("/", (int custId, int subAccId, ICustomerOtpService request2) => HandleAsync(custId, subAccId, request2))
                .Produces<CustomerOtp>();
        }

        public async Task<Result> HandleAsync(ChangeOtpStatusParam request1, ICustomerOtpService request2)
        {
            return await request2.ChangeOtpStatus(request1);
        }

        public async Task<CustomerOtp> HandleAsync(CustomerOtpParam request1, ICustomerOtpService request2)
        {
            return await Task.FromResult(request2.GenerateCustomerOtp(request1.CustId, request1.Username, request1.SubAccId));
        }

        public async Task<bool> HandleAsync(CustomerOtpParamBase request1, ICustomerOtpService request2)
        {
            return await request2.IsEnabledOtp(request1.CustId, request1.SubAccId);
        }

        public async Task<Result> HandleAsync(CustomerOtp request1, ICustomerOtpService request2)
        {
            return await request2.SetUpOtp(request1);
        }

        public async Task<bool> HandleAsync(CustomerOtpDevice request1, ICustomerOtpService request2)
        {
            return await request2.IsDifferenceDevice(request1.CustId, request1.DeviceId, request1.SubAccId);
        }

        public async Task<Result> HandleAsync(RemoveSecretKeyParam request1, ICustomerOtpService request2)
        {
            return await request2.RemoveSecretKey(request1);
        }

        public async Task<CustomerOtp> HandleAsync(int custId, int subAccId, ICustomerOtpService request3)
        {
            return await request3.GetCustomerOtp(custId, subAccId);
        }
    }
}