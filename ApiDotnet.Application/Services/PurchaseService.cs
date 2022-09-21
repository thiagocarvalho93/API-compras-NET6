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
        private readonly IUnityOfWork _unitOfWork;

        public PurchaseService(IProductRepository productRepository, IPersonRepository personRepository, IPurchaseRepository purchaseRepository, IMapper mapper, IUnityOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _personRepository = personRepository;
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultService<PurchaseDTO>> CreateAsync(PurchaseDTO purchaseDTO)
        {
            if (purchaseDTO == null)
                return ResultService.Fail<PurchaseDTO>("Objeto deve ser informado.");

            var validation = new PurchaseDTOValidator().Validate(purchaseDTO);
            if (!validation.IsValid)
                return ResultService.RequestError<PurchaseDTO>("Há campos inválidos.", validation);


            try
            {
                await _unitOfWork.BeginTransaction();
                var productId = await _productRepository.GetIdByCodErpAsync(purchaseDTO.CodErp);
                if (productId == 0)
                {
                    var product = new Product(purchaseDTO.ProductName, purchaseDTO.CodErp, purchaseDTO.Price ?? 0);
                    product = await _productRepository.CreateAsync(product);
                    productId = product.Id;
                }
                var personId = await _personRepository.GetIdByDocumentAsync(purchaseDTO.Document);

                var purchase = new Purchase(productId, personId);

                var data = await _purchaseRepository.CreateAsync(purchase);
                purchaseDTO.Id = data.Id;
                await _unitOfWork.Commit();
                return ResultService.Ok<PurchaseDTO>(purchaseDTO);
            }
            catch (System.Exception ex)
            {
                await _unitOfWork.Rollback();
                return ResultService.Fail<PurchaseDTO>(ex.Message);
            }
        }

        public async Task<ResultService> DeleteAsync(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null)
                return ResultService.Fail("Compra não encontrada.");

            await _purchaseRepository.DeleteAsync(purchase);
            return ResultService.Ok($"Compra {id} deletada com sucesso.");
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

        public async Task<ResultService<PurchaseDTO>> UpdateAsync(PurchaseDTO purchaseDTO)
        {
            if (purchaseDTO == null)
                return ResultService.Fail<PurchaseDTO>("Insira um objeto.");

            var result = new PurchaseDTOValidator().Validate(purchaseDTO);
            if (!result.IsValid)
                return ResultService.RequestError<PurchaseDTO>("Há campos inválidos", result);

            var purchase = await _purchaseRepository.GetByIdAsync(purchaseDTO.Id);
            // TODO Retornar 404 no controller
            if (purchase == null)
                return ResultService.Fail<PurchaseDTO>("Compra não encontrada.");

            var productId = await _productRepository.GetIdByCodErpAsync(purchaseDTO.CodErp);
            if (productId == 0)
                return ResultService.Fail<PurchaseDTO>("Código Erp não encontrado.");

            var personId = await _personRepository.GetIdByDocumentAsync(purchaseDTO.Document);
            if (personId == 0)
                return ResultService.Fail<PurchaseDTO>("Documento não encontrado.");

            purchase.Edit(purchase.Id, productId, personId);

            await _purchaseRepository.EditAsync(purchase);
            return ResultService.Ok<PurchaseDTO>(purchaseDTO);
        }
    }
}