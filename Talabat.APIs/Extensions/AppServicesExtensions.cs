using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.IRepositories;
using Talabat.Core.Service;
using Talabat.Core.UOW_Interface;
using Talabat.Repo.Repositories;
using Talabat.Repo.UOW;
using Talabat.Services.OrderServices;
using Talabat.Services.Payment;

namespace Talabat.APIs.Extensions
{
    public static class AppServicesExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection Services)
        {
            //Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            Services.AddScoped(typeof(IBasketRepo), typeof(BasketRepo));
            Services.AddScoped(typeof(IOrderService), typeof(OrderService));
            Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));

            Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            Services.AddAutoMapper(typeof(MappingProfiles));

            // Error Handling
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actioncontext) =>
                {
                    var Errs = actioncontext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                       .SelectMany(p => p.Value.Errors)
                                                       .Select(E => E.ErrorMessage).ToList();
                    var ValidationErrRespons = new ApiValidationError()
                    {
                        Errors = Errs
                    };
                    return new BadRequestObjectResult(ValidationErrRespons);
                };
            });
            return Services;
        }
    }
}
