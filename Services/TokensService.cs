using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Contracts;
using Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.IdentityModel.Tokens.Jwt;

namespace Services
{
    public class TokensService: ITokensService
    {
        private readonly ILogger<TokensService> _logger;
        private readonly TokensServiceOptions _options;
        public TokensService(ILogger<TokensService> logger, IOptions<TokensServiceOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public string JwtToken(List<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var signCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = signCredentials,
                Audience = _options.Audience,
                Issuer = _options.Issuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(securityToken);
            
            return tokenString;
        }
    }
}
