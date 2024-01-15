using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.IRepositories;
using Talabat.Core.Service;
using Talabat.Core.Specifications.Order_Spec;
using Talabat.Core.UOW_Interface;

namespace Talabat.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepo _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepo basketRepo, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);
            var orderItems = new List<OrderItem>();
            if(basket?.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                    var orederItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
                    orderItems.Add(orederItem);
                }
            }
            var SubTotal = orderItems.Sum(item => item.Price * item.Quantity);
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId);
            var spec = new OrderWithPaymentspec(basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if(ExOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
            }
            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, SubTotal, basket.PaymentIntentId);
            await _unitOfWork.Repository<Order>().Add(order);
            var Result = await _unitOfWork.CompleteAsync();
            if (Result <= 0) return null;
            return order;
        }

        public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecifications(buyerEmail, orderId);
            var Order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            return Order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return orders;
        }
    }
}
