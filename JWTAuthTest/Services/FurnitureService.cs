using JWTAuthTest.Data;
using JWTAuthTest.Entities;
using JWTAuthTest.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JWTAuthTest.Services
{
    public interface IFurnitureService
    {
        Task CreateFurniture(Furniture furniture);
        Task<IEnumerable<Furniture>> GetAllFurniture();
        Task<Furniture> GetFurnitureById(int id);
        Task<Furniture> DeleteFurniture(int id);
    }
    public class FurnitureService : IFurnitureService
    {
        private readonly SQLDbContext _context;
        private readonly AppSettings _appSettings;

        public FurnitureService(SQLDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }
        public async Task CreateFurniture(Furniture furniture)
        {
            if (furniture == null) throw new Exception("Debes llenar todos los campos obligatorios para continuar");
            furniture = EncryptFurniture(furniture);
            _context.Furnitures.Add(furniture);
            await _context.SaveChangesAsync();
        }

        public Task<Furniture> DeleteFurniture(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Furniture>> GetAllFurniture()
        {
            var furnitures = await _context.Furnitures.ToListAsync();
            if (furnitures == null) throw new Exception("No se encontro ningun producto en el sistema");
            furnitures.ForEach(furniture =>
            {
                furniture = DecryptFurniture(furniture);
            });
            return furnitures;
        }

        public Task<Furniture> GetFurnitureById(int id)
        {
            throw new NotImplementedException();
        }

        private Furniture EncryptFurniture(Furniture furniture)
        {
            var key = _appSettings.AESKey;
            var aes = new AESOperation();
            furniture.Name = aes.Encrypt(key, furniture.Name);
            furniture.Price = aes.Encrypt(key, furniture.Price);
            return furniture;
        }
        private Furniture DecryptFurniture(Furniture furniture)
        {
            var key = _appSettings.AESKey;
            var aes = new AESOperation();
            furniture.Name = aes.Decrypt(key, furniture.Name);
            furniture.Price = aes.Decrypt(key, furniture.Price);
            return furniture;
        }
    }
}
