using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Main.Defaults
{
    public class AuthConfig
    {
        public const string Position = "Authentication";
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int LifeTime { get; set; }

        public SymmetricSecurityKey SymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}
