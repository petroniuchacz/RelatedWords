using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using RelatedWordsAPI.App;
using RelatedWordsAPI.App.Helpers;
using RelatedWordsAPI.Models;
using System.Threading.Tasks;

namespace RelatedWordsAPI.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task Register(User user);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications

        private readonly AppSettings _appSettings;
        private readonly RelatedWordsContext _context;

        public UserService(IOptions<AppSettings> appSettings, RelatedWordsContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public async Task<User> Authenticate(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email).ConfigureAwait(false);

            // return null if user not found
            if (user == null)
                return null;

            // Validate password
            if (PasswordHash.Hash(password, user.Salt) != user.PasswordHash)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(_appSettings.TokenExpiryInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user;
        }

        public async Task Register(User user)
        {
            Tuple<string, string> saltAndPass = PasswordHash.GenerateSaltAndHash(user.Password);
            user.Salt = saltAndPass.Item1; user.PasswordHash = saltAndPass.Item2;

            _context.Add(user);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            await _context.Users.LoadAsync().ConfigureAwait(false);
            return _context.Users.ToList();
        }

        public async Task<User> GetById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id).ConfigureAwait(false);
                
            return user;
        }
    }
}