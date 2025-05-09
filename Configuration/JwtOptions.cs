using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using System.Text;

namespace UserEntityWebAPI.Configuration
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience {  get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int LifetimeMinutes { get; set; } = 10;
    }
}
