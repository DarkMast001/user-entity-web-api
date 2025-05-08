using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using System.Text;

namespace UserEntityWebAPI
{
    // Поместить его в отдельный файл или папку Configuration.
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience {  get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int LifetimeMinutes { get; set; } = 10;
    }
}
