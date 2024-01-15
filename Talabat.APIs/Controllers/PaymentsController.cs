using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Service;

namespace Talabat.APIs.Controllers
{

    public class PaymentsController : APIBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        const string endpointSecret = "whsec_ae66c5712de85dcf223a82a8fecfe737bad7615f72be36d6928f33bdee3336b2";


        public PaymentsController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(CustomerBasketDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDTO>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var CustomerBasket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (CustomerBasket == null) return BadRequest(new ApiResponse(400, "There is a Problem With Your Basket"));
            var MappedBasket = _mapper.Map<CustomerBasketDTO>(CustomerBasket);
            return Ok(MappedBasket);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, false);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, true);

                }


                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
