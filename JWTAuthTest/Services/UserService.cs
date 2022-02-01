using JWTAuthTest.Data;
using JWTAuthTest.Entities;
using JWTAuthTest.Models;
using JWTAuthTest.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthTest.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
        Task<AuthenticateResponse> CreateUser(User user);
        Task<bool> SaveChanges();
    }
    public class UserService : IUserService
    {
        
        private readonly SQLDbContext _context;
        private readonly AppSettings _appSettings;
        public UserService(IOptions<AppSettings> appSettings, SQLDbContext context)
        {
            _appSettings = appSettings.Value;  
            _context = context;
        }
        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var aes = new AESOperation();
            var key = _appSettings.AESKey;
            model.Name = aes.Encrypt(key, model.Name);
            model.Password = aes.Encrypt(key, model.Password);
            var user = _context.Users.SingleOrDefault(x => x.Name == model.Name && x.Password == model.Password);
            if(user == null) return null;
            var token = GenerateJwtToken(user);
            var authUser = new AuthenticateResponse(user, token);
            authUser.Name = aes.Decrypt(key, authUser.Name);
            authUser.Email = aes.Decrypt(key, authUser.Email);
            return authUser;
        }

        public IEnumerable<User> GetAll()
        {
            var key = _appSettings.AESKey;
            var aes = new AESOperation();
            var users = _context.Users.ToList();
            if (users == null) throw new FileNotFoundException("No se encuentran usuarios en el sistema");
            foreach(var user in users)
            {
                user.Name = aes.Decrypt(key, user.Name);
                user.Email = aes.Decrypt(key, user.Email);
            }
            return users;
        }

        public User GetById(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) throw new Exception("El usuario no se encuentra registrado en el sistema");        
            return user;
        }
        public async Task<AuthenticateResponse> CreateUser(User user)
        {
            var aes = new AESOperation();
            var key = _appSettings.AESKey;
            if(user == null) throw new ArgumentNullException(nameof(user));
            user.Name = aes.Encrypt(key, user.Name);
            user.Password = aes.Encrypt(key, user.Password);
            user.Email = aes.Encrypt(key, user.Email);
            await _context.Users.AddAsync(user);
            var token = GenerateJwtToken(user);
            return new AuthenticateResponse(user, token);
        }
        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync() > 1);
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new [] { new Claim("id", user.Id.ToString()) } ),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        
    }
}
