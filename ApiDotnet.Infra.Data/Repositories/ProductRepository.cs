using ApiDotnet.Domain.Entities;
using ApiDotnet.Domain.Repositories;
using ApiDotnet.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiDotnet.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _db.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(Product product)
        {
            _db.Remove(product);
            await _db.SaveChangesAsync();
        }

        public async Task EditAsync(Product product)
        {
            _db.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task<Product> GetByIdAsync(int id) => await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
        public async Task<int> GetIdByCodErpAsync(string codErp) => (await _db.Products.FirstOrDefaultAsync(x => x.CodErp == codErp))?.Id ?? 0;
        public async Task<ICollection<Product>> GetProductsAsync() => await _db.Products.ToListAsync();

    }
}