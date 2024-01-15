using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.DTOs
{
    public class OrderToReturnDTO
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } 
        public Address ShippingAddress { get; set; }
        public string Status { get; set; } 
        public string DeliveryMethod { get; set; }
        public decimal DeliveryMethodCost { get; set; }
        public ICollection<OrderItemDTO> Items { get; set; } = new HashSet<OrderItemDTO>();
        public decimal SupTotal { get; set; }
        public decimal Total { get; set; }
        public string PaymentIntentId { get; set; }

    }
}
