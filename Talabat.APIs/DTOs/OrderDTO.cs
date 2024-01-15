using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
    public class OrderDTO
    {
        [Required]
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public AddressDTO ShippingAddress { get; set; }
    }
}
