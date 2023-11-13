using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NorthWindRestApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NorthWindRestApi.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly NorthwindOriginalContext db;
        private readonly AppSettings _appSettings;
        public AuthenticateService(IOptions<AppSettings> appSettings, NorthwindOriginalContext nwc)
        {
            _appSettings = appSettings.Value;
            db = nwc;
        }

        public LoggedUser? Authenticate(string username, string password)
        {
            var foundUser = db.Users.SingleOrDefault(x => x.UserName == username);

            if (foundUser == null || !BCrypt.Net.BCrypt.Verify(password, foundUser.Password))
            {
                return null;
            }

            //if (foundUser == null)
            //{
            //    var l = new LoggedUser();
            //    l.Username = "Nolla";
            //    l.AccesslevelId = 1;
            //    l.Token = null;
            //    return l;
            //}
            //if (!BCrypt.Net.BCrypt.Verify(password, foundUser.Password))
            //{
            //    var l = new LoggedUser();
            //    l.Username = "Virhe";
            //    l.AccesslevelId = 1;
            //    l.Token = null;
            //    return l;
            //}

            // Jos käyttäjä löytyy:
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, foundUser.UserId.ToString()),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.Version, "V3.1")
                }),
                Expires = DateTime.UtcNow.AddDays(1), // Montako päivää token on voimassa

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoggedUser loggedUser = new LoggedUser();

            loggedUser.Username = foundUser.UserName;
            loggedUser.AccesslevelId = foundUser.AccessLevelid;
            loggedUser.Token = tokenHandler.WriteToken(token);

            return loggedUser; // Palautetaan kutsuvalle controllerimetodille user ilman salasanaa
        }
    }
}
