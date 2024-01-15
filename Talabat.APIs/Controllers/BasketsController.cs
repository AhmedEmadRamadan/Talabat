using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;

namespace Talabat.APIs.Controllers
{
    public class BasketsController : APIBaseController
    {
        private readonly IBasketRepo _basketRepo;
        private readonly IMapper _mapper;

        public BasketsController(IBasketRepo basketRepo, IMapper mapper)
        {
            _basketRepo = basketRepo;
            _mapper = mapper;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string id)
        {
            var basket = await _basketRepo.GetBasketAsync(id);
            return basket is null ? new CustomerBasket(id) : basket;
        }
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDTO basket)
        {
            var MappedBasket = _mapper.Map<CustomerBasketDTO, CustomerBasket>(basket);
            var CreatedOrUpdated =await _basketRepo.UpdateBasketAsync(MappedBasket);
            return CreatedOrUpdated is null ? BadRequest(new ApiResponse(400, "Ther Is A Problem With Your Basket")) : Ok(CreatedOrUpdated);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            return await _basketRepo.DeleteBasketAsync(id);
        }
    }
}
