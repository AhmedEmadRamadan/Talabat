using Microsoft.Extensions.Configuration;
using Stripe;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.IRepositories;
using Talabat.Core.Service;
using Talabat.Core.Specifications.Order_Spec;
using Talabat.Core.UOW_Interface;

namespace Talabat.Services.Payment
{
    public class PaymentService : IPaymentService
    {   //
        private readonly IConfiguration _configuration;
        private readonly IBasketRepo _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration, IBasketRepo basketRepo, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            // secret key
            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
            //get basket
            var Basket = await _basketRepo.GetBasketAsync(basketId);
            if (Basket == null) return null;
            var ShippingPrice = 0M;
            if (Basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(Basket.DeliveryMethodId.Value);
                ShippingPrice = deliveryMethod.Cost;
            }
            if(Basket.Items.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var product = await _unitOfWork.Repository<Core.Entities.Product>().GetAsync(item.Id);
                    if(item.Price != product.Price) 
                        item.Price = product.Price;
                }
            }
            var subTotal = Basket.Items.Sum(item =>  item.Price * item.Quantity);
            //create paymentintent
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if(string.IsNullOrEmpty(Basket.PaymentIntentId))
            {
                var Options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(subTotal*100 + ShippingPrice*100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent = await service.CreateAsync(Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(subTotal * 100 + ShippingPrice * 100),
                };
                paymentIntent = await service.UpdateAsync(Basket.PaymentIntentId, Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            await _basketRepo.UpdateBasketAsync(Basket);
            return Basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string paymentIntentId, bool flag)
        {
            var spec = new OrderWithPaymentspec(paymentIntentId);
            var Order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if (flag)
                Order.Status = OrderStatus.PaymentRecieved;
            else
                Order.Status = OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(Order);
            await _unitOfWork.CompleteAsync();
            return Order;
        }
    }
}


