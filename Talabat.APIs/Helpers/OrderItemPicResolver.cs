using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.Helpers
{
    public class OrderItemPicResolver : IValueResolver<OrderItem, OrderItemDTO, string>
    {
        private readonly IConfiguration _config;
        public OrderItemPicResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(OrderItem source, OrderItemDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Product.PictureUrl))
                return $"{_config["ApiBaseUrl"]}{source.Product.PictureUrl}";
            return string.Empty;
        }
    }
}
