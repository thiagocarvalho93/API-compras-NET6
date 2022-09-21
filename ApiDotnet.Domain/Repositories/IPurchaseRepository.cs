using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDotnet.Domain.Entities;

namespace ApiDotnet.Domain.Repositories
{
    public interface IPurchaseRepository
    {
        Task<Purchase> GetByIdAsync(int id);
        Task<ICollection<Purchase>> GetAllAsync();
        Task<Purchase> CreateAsync(Purchase purchase);
        Task EditAsync(Purchase purchase);
        Task DeleteAsync(Purchase purchase);
        Task<ICollection<Purchase>> GetByPersonIdAsync(int personId);
        Task<ICollection<Purchase>> GetByProductIdAsync(int productId);
    }
}