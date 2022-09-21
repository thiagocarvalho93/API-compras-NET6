using ApiDotnet.Application.DTOs;
using ApiDotnet.Application.DTOs.Validations;
using ApiDotnet.Application.Services.Interfaces;
using ApiDotnet.Domain.Entities;
using ApiDotnet.Domain.Repositories;
using AutoMapper;

namespace ApiDotnet.Application.Services
{
    public class PurchaseService : IPurchaseService
    {

        private readonly IProductRepository _productRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMapper _mapper;

        public PurchaseService(IProductRepository productRepository, IPersonRepository personRepository, IPurchaseRepository purchaseRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _personRepository = personRepository;
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
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

        public async Task<ResultService<ICollection<PurchaseDetailDTO>>> GetAsync()
        {
            var result = await _purchaseRepository.GetAllAsync();
            return ResultService.Ok<ICollection<PurchaseDetailDTO>>(_mapper.Map<ICollection<PurchaseDetailDTO>>(result));
        }

        public async Task<ResultService<PurchaseDetailDTO>> GetByIdAsync(int id)
        {
            var result = await _purchaseRepository.GetByIdAsync(id);
            if (result == null)
                return ResultService.Fail<PurchaseDetailDTO>("Compra não encontrada.");
            return ResultService.Ok<PurchaseDetailDTO>(_mapper.Map<PurchaseDetailDTO>(result));
        }

        public Task<ResultService> UpdateAsync(PurchaseDTO purchaseDTO)
        {
            throw new NotImplementedException();
        }
    }
}