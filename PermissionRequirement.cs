using Microsoft.AspNetCore.Authorization;

namespace PolicyBasedAuthentication
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string apiKey, string secretKey)
        {
            SecretKey = secretKey;
            ApiKey = apiKey;
        }
        public string ApiKey { get; set; }
        public string SecretKey { get; }
    }
}
