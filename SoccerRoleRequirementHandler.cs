using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PolicyBasedAuthentication
{
    public class SoccerRequirement : IAuthorizationRequirement
    {
        public SoccerRequirement(string apiKey, string secretKey)
        {
            SecretKey = secretKey;
            ApiKey = apiKey;
        }
        public string ApiKey { get; set; }
        public string SecretKey { get; }
    }
    public class SoccerRoleRequirementHandler : AuthorizationHandler<SoccerRequirement>
    {
        private const string API_KEY_HEADER_NAME = "X-API-KEY";
        private const string SECRET_KEY_HEADER_NAME = "X-SECRET-KEY";
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SoccerRequirement requirement)
        {
            CheckApiKey(context, requirement);
            return Task.CompletedTask;
        }

        private Task CheckApiKey(AuthorizationHandlerContext context, SoccerRequirement requirement)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var apiKey = httpContext.Request.Headers[API_KEY_HEADER_NAME].FirstOrDefault();
                var secretKey = httpContext.Request.Headers[SECRET_KEY_HEADER_NAME].FirstOrDefault();
                if (apiKey is null || apiKey != requirement.ApiKey || secretKey is null || secretKey != requirement.SecretKey)
                {
                    context.Fail();
                    SendHttpResponseMessage(httpContext, "test message");
                    return Task.CompletedTask;
                }
                else
                {
                    var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(secretKey);
                    var role = jwtSecurityToken.Claims.First(claim => claim.Type == "Role").Value;
                    if (role == "Soccer")
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                       
                    else
                    {
                        context.Fail();
                        SendHttpResponseMessage(httpContext, "test message");
                        
                    }
                }

                context.Fail();
                SendHttpResponseMessage(httpContext, "test message");
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
        private void SendHttpResponseMessage(HttpContext httpContext, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            httpContext.Response.StatusCode = 401;
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
