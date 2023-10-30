using System.ComponentModel.DataAnnotations;

namespace WEB_API.Models.Orders_model
{
    public class OrderModel
    {
        [Required]
        public int userId { get; set; }
        [Required]
        public string ShipAddress { get; set; }
        [Required]
        public int cityShipId { get; set; }
        [Required]
        public string tel { get; set; }
        [Required]
        public int paymentMethodId { get; set; }

    }
}

// userId     ShipAddress    cityShip    tel    paymentMethodId