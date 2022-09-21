using ApiDotnet.Application.DTOs;

namespace ApiDotnet.Application.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<ResultService<PurchaseDTO>> CreateAsync(PurchaseDTO purchaseDTO);
        Task<ResultService<ICollection<PurchaseDTO>>> GetAsync();
        Task<ResultService<PurchaseDTO>> GetByIdAsync(int id);
        Task<ResultService> UpdateAsync(PurchaseDTO purchaseDTO);
        Task<ResultService> DeleteAsync(int id);
    }
}