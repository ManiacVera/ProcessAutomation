using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Framework.DTO.Constants;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace ProcessAutomation.Business.Attribute
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizationRequiredAttribute : ActionFilterAttribute
    {
        //private readonly IAccessTokenServices accessTokenServices;
        //private readonly IPermissionServices permissionServices;
        //private readonly IAuditLogServices auditLogServices;
        private readonly ISecurityBM securityBM;
        private IConfiguration config;

        public AuthorizationRequiredAttribute(ISecurityBM securityBM, IConfiguration config)
        {
            this.securityBM = securityBM;
            this.config = config;
            //this.accessTokenServices = accessTokenServices;
            //this.permissionServices = permissionServices;
            //this.auditLogServices = auditLogServices;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                if (context.HttpContext.Request.Headers.ContainsKey(FrameworkConstants.PROCESS_AUTOMATION_TOKEN))
                {
                    var handler = new JwtSecurityTokenHandler();
                    string token = context.HttpContext.Request.Headers[FrameworkConstants.PROCESS_AUTOMATION_TOKEN];
                    ContextUserInfo contextUserInfo = securityBM.GetContextToken(context.HttpContext.Request.Headers[FrameworkConstants.PROCESS_AUTOMATION_TOKEN]);

                    //var securityToken = handler.ReadToken(context.HttpContext.Request.Headers[FrameworkConstants.PROCESS_AUTOMATION_TOKEN])
                    //    as JwtSecurityToken;

                    if (contextUserInfo.ExpirationDate < DateTime.Now)
                    {
                        securityBM.DeleteToken(token);
                        context.Result = new UnauthorizedResult();
                    }

                    //if (context.HttpContext.Request.Method == WebRequestMethods.Http.Get)
                    //{
                    //    if (expireDate < DateTime.Now)
                    //    {
                    //        context.Result = new UnauthorizedResult();
                    //    }
                    //}
                    //else
                    //{

                    //    var accessToken = accessTokenServices
                    //        .Details(x => x.Token == context.HttpContext.Request.Headers[Constants.HttpHeaders.Token]);
                    //    if (accessToken != null)
                    //    {
                    //        if (accessToken.ExpiresOn < DateTime.Now)
                    //        {
                    //            accessTokenServices.Delete(accessToken);
                    //            context.Result = new UnauthorizedResult();
                    //        }
                    //        else
                    //        {
                    //            var userId = Convert.ToInt32(token.Claims.First(claim => claim.Type == Constants.JwtClaims.UserId).Value);
                    //            var userTypeId = Convert.ToInt32(token.Claims.First(claim => claim.Type == Constants.JwtClaims.UserTypeId).Value);
                    //            if (accessToken == null)
                    //            {
                    //                context.Result = new UnauthorizedResult();
                    //            }
                    //            else if (!permissionServices.IsPermissionExist(context.HttpContext.Request.Path.ToString(), userTypeId))
                    //            {
                    //                context.Result = new StatusCodeResult((int)HttpStatusCode.NotAcceptable);
                    //            }
                    //            else
                    //            {

                    //                auditLogServices.Save(context.HttpContext.Request.Path.ToString(), userId);
                    //                accessToken.ExpiresOn = DateTime.Now.AddMinutes(Convert.ToInt16(config["Jwt:ExpiresOn"]));
                    //                accessTokenServices.UpdateExpireTime(accessToken);

                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        context.Result = new UnauthorizedResult();
                    //    }
                    //}
                }
                else
                {
                    context.Result = new NotFoundResult();
                }
            }
            catch (Exception)
            {
                context.Result = new BadRequestResult();
            }

            base.OnActionExecuting(context);
        }
    }
}
