using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Specifications.Order_Spec
{
    public class OrderWithPaymentspec : BaseSpecifications<Order>
    {
        public OrderWithPaymentspec(string paymentIntentId):base(o => o.PaymentIntentId == paymentIntentId)
        {
            
        }
    }
}
