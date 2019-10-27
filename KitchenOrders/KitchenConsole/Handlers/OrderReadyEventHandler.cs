using System.Threading.Tasks;
using JustSaying.Messaging.MessageHandling;
using KitchenOrders.Messages;
using Microsoft.Extensions.Logging;

namespace KitchenOrders.KitchenConsole.Handlers
{
    public class OrderReadyEventHandler : IHandlerAsync<OrderReadyEvent>
    {
        private readonly ILogger<OrderReadyEventHandler> _log;

        public OrderReadyEventHandler(ILogger<OrderReadyEventHandler> log)
        {
            _log = log;
        }

        public Task<bool> Handle(OrderReadyEvent message)
        {
            // This subscription isn't needed but just proves we can have multiple subscribers
            _log.LogInformation("Why am I telling myself that order {orderId} is ready!", message.OrderId);
            return Task.FromResult(true);
        }
    }
}
