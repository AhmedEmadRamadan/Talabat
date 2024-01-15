using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Service;
using Talabat.Core.UOW_Interface;
using Talabat.Services.OrderServices;

namespace Talabat.APIs.Controllers
{
    public class OrdersController : APIBaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [ProducesResponseType(typeof(Core.Entities.OrderAggregate.Order), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Core.Entities.OrderAggregate.Order>> CreateOrder(OrderDTO orderDTO)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var MappedAddress = _mapper.Map<AddressDTO, Address>(orderDTO.ShippingAddress);
            var Order = await _orderService.CreateOrderAsync(BuyerEmail, orderDTO.BasketId, orderDTO.DeliveryMethodId, MappedAddress);
            if (Order == null) return BadRequest(new ApiResponse(400, "There is a Problem With Your Order"));
            return Ok(Order);
        }


        [ProducesResponseType(typeof (IReadOnlyList<OrderToReturnDTO>), 200)]
        [ProducesResponseType(typeof (ApiResponse), 404)]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDTO>>> GetOrders()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var Orders = await _orderService.GetOrdersForSpecificUserAsync(userEmail);
            if (Orders == null) return NotFound(new ApiResponse(404, "There Is No Orders For This User"));
            var MappedOrder = _mapper.Map<IReadOnlyList<Core.Entities.OrderAggregate.Order>, IReadOnlyList<OrderToReturnDTO>>(Orders);
            return Ok(MappedOrder);
        }
        [ProducesResponseType(typeof(OrderToReturnDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<OrderToReturnDTO>> GetOrderById(int id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var OrderById = await _orderService.GetOrderByIdForSpecificUserAsync(userEmail, id);
            if (OrderById == null) return NotFound(new ApiResponse(404, $"There is No Order with id : {id}"));
            var MappedOrder = _mapper.Map<Core.Entities.OrderAggregate.Order, OrderToReturnDTO>(OrderById);
            return Ok(MappedOrder);
        }

        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<DeliveryMethod>> GetDeliveryMethods()
        {
            var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return Ok(DeliveryMethods);
        }
    }
}
