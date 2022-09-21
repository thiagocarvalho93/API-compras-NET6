using ApiDotnet.Application.DTOs;
using ApiDotnet.Domain.FiltersDb;

namespace ApiDotnet.Application.Services.Interfaces
{
    public interface IPersonService
    {
        Task<ResultService<PersonDTO>> CreateAsync(PersonDTO personDTO);
        Task<ResultService<ICollection<PersonDTO>>> GetAsync();
        Task<ResultService<PersonDTO>> GetByIdAsync(int id);
        Task<ResultService> UpdateAsync(PersonDTO personDTO);
        Task<ResultService> DeleteAsync(int id);
        Task<ResultService<PageBasedResponseDTO<PersonDTO>>> GetPagedAsync(PersonFilterDb personFilterDb);
    }
}