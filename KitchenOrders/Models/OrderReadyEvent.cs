using JustSaying.Models;

namespace KitchenOrders.Messages
{
    public class OrderReadyEvent : Message
    {
        public int OrderId { get; set; }
    }
}
