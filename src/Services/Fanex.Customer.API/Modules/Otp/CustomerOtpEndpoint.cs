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

            otp.MapPost("/change-status", (ChangeOtpStatusParam request, ICustomerOtpService service) => HandleAsync(request, service))
                .Produces<Result>();

            otp.MapPost("/generate", (CustomerOtpParam request, ICustomerOtpService service) => HandleAsync(request, service))
                .Produces<CustomerOtp>();

            otp.MapPost("/isenabled", (CustomerOtpParamBase request, ICustomerOtpService service) => HandleAsync(request, service))
                .Produces<bool>();

            otp.MapPost("/setup", (CustomerOtp request, ICustomerOtpService service) => HandleAsync(request, service))
                .Produces<Result>();

            otp.MapPost("/isdifferent-device", (CustomerOtpDevice request, ICustomerOtpService service) => HandleAsync(request, service))
                .Produces<bool>();

            otp.MapPost("/remove", (RemoveSecretKeyParam request, ICustomerOtpService service) => HandleAsync(request, service))
                .Produces<Result>();
            
            otp.MapGet("/", (int custId, int subAccId, ICustomerOtpService service) => HandleAsync(custId, subAccId, service))
                .Produces<CustomerOtp>();
        }

        public async Task<Result> HandleAsync(ChangeOtpStatusParam request, ICustomerOtpService service)
        {
            return await service.ChangeOtpStatus(request);
        }

        public async Task<CustomerOtp> HandleAsync(CustomerOtpParam request, ICustomerOtpService service)
        {
            return await Task.FromResult(service.GenerateCustomerOtp(request.CustId, request.Username, request.SubAccId));
        }

        public async Task<bool> HandleAsync(CustomerOtpParamBase request, ICustomerOtpService service)
        {
            return await service.IsEnabledOtp(request.CustId, request.SubAccId);
        }

        public async Task<Result> HandleAsync(CustomerOtp request, ICustomerOtpService service)
        {
            return await service.SetUpOtp(request);
        }

        public async Task<bool> HandleAsync(CustomerOtpDevice request, ICustomerOtpService service)
        {
            return await service.IsDifferenceDevice(request.CustId, request.DeviceId, request.SubAccId);
        }

        public async Task<Result> HandleAsync(RemoveSecretKeyParam request, ICustomerOtpService service)
        {
            return await service.RemoveSecretKey(request);
        }

        public async Task<CustomerOtp> HandleAsync(int custId, int subAccId, ICustomerOtpService service)
        {
            return await service.GetCustomerOtp(custId, subAccId);
        }
    }
}