using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GCCars.Application.Settings
{
    public class AuthSettings
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Key { get; set; }

        public int Lifetime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.ASCII.GetBytes(Key));
    }
}
