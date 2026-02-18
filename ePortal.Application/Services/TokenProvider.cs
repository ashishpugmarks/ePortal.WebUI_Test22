using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ePortal.Application.Services
{
    public class TokenProvider : ITokenProvider
    {
        // Ideally, store this securely or get it from login/session
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ISessionService _session;

        public TokenProvider(IHttpContextAccessor httpContextAccessor, IConfiguration config, ISessionService session)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = config;
            _session = session;
        }

        public Task<string> GetTokenAsync()
        {
            // Assume token is stored in session or cookie
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            return Task.FromResult(token ?? string.Empty);
        }


        public async Task<string> GetOrCreateTokenAsync()
        {
            var user = _session.Get<string>("userID");

            if (user == null) { throw new UnauthorizedAccessException("User is not logged in."); }

            var token = _session.Get<string>("JWToken");
            var expiry = _session.Get<DateTime>("ApiTokenExpiry");

            if (string.IsNullOrEmpty(token) || expiry < DateTime.UtcNow)
            {
                var newToken = await GenerateTokenAsync(user);
                _session.Set("JWToken", newToken.Token);
                _session.Set("ApiTokenExpiry", newToken.Expiry);
                return newToken.Token;
            }

            return token;
        }

        private Task<(string Token, DateTime Expiry)> GenerateTokenAsync(string userName)
        {
            // Logic to create JWT with short expiration (e.g., 10 mins)
            // based on user's claims

            // Create claims for the JWT
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("userid", userName)
            };

            // Generate signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Calculate token expiration time
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]));
            //var expirationTime = DateTime.UtcNow.AddDays(1);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expirationTime, // Use the calculated expiration time
                signingCredentials: creds);

            // Generate the JWT string
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);            

            return Task.FromResult((jwtToken,expirationTime)); // Return the JWT token
        }
    }
}
