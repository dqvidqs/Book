using BookAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace BookAPI.Auth
{
    public class TokenProvider
    {
        private readonly IConfiguration configuration;
        public TokenProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string LoginUser(User user)
        {
            var exp = Convert.ToInt32(configuration["Jwt:ExpiryInMinutes"]);
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]));

            var JWToken = new JwtSecurityToken(
                issuer: configuration["Jwt:Site"],
                audience: configuration["Jwt:Site"],
                claims: GetUserClaims(user),
                //notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: DateTime.UtcNow.AddMinutes(exp),
                signingCredentials: new SigningCredentials
                (key, SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            return token;
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            IEnumerable<Claim> claims = new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.name),
                    new Claim("USERID", user.id.ToString()),
                    new Claim("EMAILID", user.email),
                    new Claim("ACCESS_LEVEL", user.role.ToUpper()),
                    };
            return claims;
        }
        public string Hash(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }
        public bool Verify(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            int i = 0;
            //return ByteArraysEqual(buffer3, buffer4);
            foreach(byte a in buffer4)
            {
                if (buffer3[i++] != a) { return false; }
            }
            return true;
        }
    }
}
