using JustSaying.Models;

namespace KitchenOrders.Messages
{
    public class OrderPlacedEvent : Message
    {
        public int OrderId { get; set; }

        public string Description { get; set; }
    }
}
