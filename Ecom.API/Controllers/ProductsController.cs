using AutoMapper;
using Ecom.API.Helper;
using Ecom.API.Mapping;
using Ecom.Core.DTO;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> get()
        {
            try
            {
                var Product = await work.ProductRepository.GetAllAsync(x => x.Category, x=> x.Photos);
                var result = mapper.Map<List<ProductDTO>>(Product);
                if (Product is null)
                {
                    return BadRequest(new ResponseAPI(400));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> getById(int id)
        {
            try
            {
                var product = await work.ProductRepository.GetByIdAsync(id, x => x.Category, x => x.Photos);
                var result = mapper.Map<ProductDTO>(product);
                if (product is null)
                {
                    return BadRequest(new ResponseAPI(400));
                }
                return Ok(result);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Add-Product")]
        public async Task<IActionResult> add(AddProductDTO productDTO)
        {
            try
            {
                await work.ProductRepository.AddAsync(productDTO);
                return Ok(new ResponseAPI(200));
            }
            catch (Exception ex) 
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        [HttpPut("Update-Product")]
        public async Task<IActionResult> Update(UpdateProductDTO updateProductDTO)
        {
            try
            {
                await work.ProductRepository.UpdateAsync(updateProductDTO);
                return Ok(new ResponseAPI(200));
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
    }
}
