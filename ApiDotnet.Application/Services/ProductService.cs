using ApiDotnet.Application.DTOs;
using ApiDotnet.Application.DTOs.Validations;
using ApiDotnet.Application.Services.Interfaces;
using ApiDotnet.Domain.Entities;
using ApiDotnet.Domain.Repositories;
using AutoMapper;

namespace ApiDotnet.Application.Services
{
    public class ProductService : IProductService
    {

        private readonly IProductRepository _productRepository;
        private IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ResultService<ProductDTO>> CreateAsync(ProductDTO productDTO)
        {
            if (productDTO == null)
                return ResultService.Fail<ProductDTO>("Produto deve ser informado.");

            var result = new ProductDTOValidator().Validate(productDTO);
            if (!result.IsValid)
                return ResultService.RequestError<ProductDTO>("Há campos inválidos.", result);

            var product = _mapper.Map<Product>(productDTO);
            var data = await _productRepository.CreateAsync(product);

            return ResultService.Ok<ProductDTO>(_mapper.Map<ProductDTO>(data));
        }

        public async Task<ResultService> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return ResultService.Fail<ProductDTO>("Produto não encontrado.");

            await _productRepository.DeleteAsync(product);
            return ResultService.Ok("Produto deletado com sucesso.");
        }

        public async Task<ResultService<ICollection<ProductDTO>>> GetAsync()
        {
            var products = await _productRepository.GetProductsAsync();
            return ResultService.Ok<ICollection<ProductDTO>>(_mapper.Map<ICollection<ProductDTO>>(products));
        }

        public async Task<ResultService<ProductDTO>> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return ResultService.Fail<ProductDTO>("Produto não encontrado.");

            return ResultService.Ok<ProductDTO>(_mapper.Map<ProductDTO>(product));
        }

        public async Task<ResultService> UpdateAsync(ProductDTO productDTO)
        {
            if (productDTO == null)
                return ResultService.Fail("Objeto deve ser informado.");
            var validation = new ProductDTOValidator().Validate(productDTO);
            if (!validation.IsValid)
                return ResultService.RequestError("Existem campos inválidos.", validation);

            var product = await _productRepository.GetByIdAsync(productDTO.Id);
            if (product == null)
                return ResultService.Fail<ProductDTO>("Produto não encontrado.");

            product = _mapper.Map<ProductDTO, Product>(productDTO, product);
            await _productRepository.EditAsync(product);
            return ResultService.Ok("Produto editado com sucesso.");
        }
    }
}