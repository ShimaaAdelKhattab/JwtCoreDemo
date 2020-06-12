using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static JwtCoreDemo.Managers.TokenManager;

namespace JwtCoreDemo.Managers
{
    public interface ITokenManager
    {
        AuthToken Generate(User user);
    }

    public class TokenManager : ITokenManager
    {
        public AuthToken Generate(User user)
        {
          
            List<Claim> claims = new List<Claim>() {
            new Claim (JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),

            new Claim (JwtRegisteredClaimNames.Email,
                user.EmailAddress),

            new Claim (JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),
		    
            // Add the ClaimType Role which carries the Role of the user
            new Claim (ClaimTypes.Role, user.Role)
        };

            JwtSecurityToken token = new TokenBuilder()
            .AddAudience(TokenConstants.Audience)
            .AddIssuer(TokenConstants.Issuer)
            .AddExpiry(TokenConstants.ExpiryInMinutes)
            .AddKey(TokenConstants.key)
            .AddClaims(claims)
            .Build();

            string accessToken = new JwtSecurityTokenHandler()
          .WriteToken(token);
            var x = new AuthToken()
            {
                AccessToken = accessToken,
                ExpiresIn = TokenConstants.ExpiryInMinutes
            };

            return new AuthToken()
            {
                AccessToken = accessToken,
                ExpiresIn = TokenConstants.ExpiryInMinutes
            };

        }
        public class TokenConstants
        {
            public static string Issuer = "thisismeyouknow";
            public static string Audience = "thisismeyouknow";
            public static int ExpiryInMinutes = 10;
            public static string key = "thiskeyisverylargetobreak";
        }
        public class AuthToken
        {
            public string AccessToken;
            public int ExpiresIn;
      
        }
    }
}
