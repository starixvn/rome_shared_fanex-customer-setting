using System;
using System.Web;
using System.Web.Mvc;
using Fanex.Customer.Client.Configurations;

namespace Fanex.Customer.Client.NETFramework.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public abstract class CustomerAuthFilterAttribute : AuthorizeAttribute
    {
        public bool IsHost { get; set; } = false;

        private readonly CustomerClientConfiguration clientConfig;

        public CustomerAuthFilterAttribute(CustomerClientConfiguration clientConfig)
        {
            this.clientConfig = clientConfig ?? CustomerClientManager.ClientConfigs;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var isAuthorized = IsHost ? AllowHost() : AllowRead();

            if (!isAuthorized)
            {
                UnAuthorizedAction(filterContext);
            }
        }

        public abstract bool AllowHost();

        public abstract bool AllowRead();

        private void UnAuthorizedAction(AuthorizationContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.IsAjaxRequest()
                || filterContext.HttpContext.Request.HttpMethod.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
            {
                response.Clear();
                response.Write("{ \"message\": \"You are not authorized on this function!\" }");
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                response.TrySkipIisCustomErrors = true;
                response.End();
            }
            else
            {
                filterContext.Result = new RedirectResult(clientConfig.AccessDeniedUrl);
            }
        }
    }
}