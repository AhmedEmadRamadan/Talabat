using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;
using Talabat.Core.Specifications;
using Talabat.Core.UOW_Interface;

namespace Talabat.APIs.Controllers
{
    
    public class ProductsController : APIBaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;


        public ProductsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts([FromQuery]ProductSpecParams Params)
        {
            var Spec = new ProductWithBrandAndTypeSpec(Params);
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
            var MappedProduct = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products);
            var CountSpec = new ProductWithFilterationForCount(Params);
            var Count = await _unitOfWork.Repository<Product>().GetCountWithSpec(CountSpec);
            return Ok(new Pagination<ProductToReturnDTO>(Params.PageSize, Params.PageIndex, MappedProduct, Count));
        }

        [ProducesResponseType(typeof(ProductToReturnDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var Spec = new ProductWithBrandAndTypeSpec(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(Spec);
            if(product == null) return NotFound(new ApiResponse(404));
            var MappedProduct = _mapper.Map<Product, ProductToReturnDTO>(product);
            return Ok(MappedProduct);
        }

        [HttpGet("Types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var Types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(Types);
        }

        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var Brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(Brands);
        }
    }
}
