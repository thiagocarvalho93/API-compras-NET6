using ApiDotnet.Application.DTOs;
using ApiDotnet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiDotnet.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            var result = await _productService.GetAsync();
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetByIdAsync(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] ProductDTO productDTO)
        {
            var result = await _productService.CreateAsync(productDTO);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // TODO Retornar 404 para id não encontrado.
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] ProductDTO productDTO)
        {
            productDTO.Id = id;
            var result = await _productService.UpdateAsync(productDTO);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
    }
}