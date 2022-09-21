using ApiDotnet.Application.DTOs;
using ApiDotnet.Application.DTOs.Validations;
using ApiDotnet.Application.Services.Interfaces;
using ApiDotnet.Domain.Entities;
using ApiDotnet.Domain.Repositories;

namespace ApiDotnet.Application.Services
{
    public class PurchaseService : IPurchaseService
    {

        private readonly IProductRepository _productRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchaseService(IProductRepository productRepository, IPersonRepository personRepository, IPurchaseRepository purchaseRepository)
        {
            _productRepository = productRepository;
            _personRepository = personRepository;
            _purchaseRepository = purchaseRepository;
        }

        public async Task<ResultService<PurchaseDTO>> CreateAsync(PurchaseDTO purchaseDTO)
        {
            if (purchaseDTO == null)
                return ResultService.Fail<PurchaseDTO>("Objeto deve ser informado.");

            var validation = new PurchaseDTOValidator().Validate(purchaseDTO);
            if (!validation.IsValid)
                return ResultService.RequestError<PurchaseDTO>("Há campos inválidos.", validation);

            var productId = await _productRepository.GetIdByCodErpAsync(purchaseDTO.CodErp);
            var personId = await _personRepository.GetIdByDocumentAsync(purchaseDTO.Document);

            var purchase = new Purchase(productId, personId);

            var data = await _purchaseRepository.CreateAsync(purchase);
            purchaseDTO.Id = data.Id;
            return ResultService.Ok<PurchaseDTO>(purchaseDTO);
        }

        public Task<ResultService> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultService<ICollection<PurchaseDTO>>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResultService<PurchaseDTO>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultService> UpdateAsync(PurchaseDTO purchaseDTO)
        {
            throw new NotImplementedException();
        }
    }
}