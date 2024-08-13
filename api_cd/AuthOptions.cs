using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace api_CodeFlow
{
    public class AuthOptions
    {
        public const string ISSUER = "https://localhost:44348"; // издатель токена
        public const string AUDIENCE = "https://localhost:44348"; // потребитель токена
        public const string KEY = "mysupersecretsecretkey!123mysupersecretsecretkey!123";   // ключ для шифрации

        public const string MyEmail = "dmitriiyuzhakov@gmail.com";
        public const string MyPassword = "yctl rtvb zrpv dtri";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
