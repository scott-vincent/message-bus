using System;
using System.Threading.Tasks;
using JustSaying.Messaging;
using JustSaying.Messaging.MessageHandling;
using KitchenOrders.Messages;
using Microsoft.Extensions.Logging;

namespace KitchenOrders.OrderingApi.Handlers
{
    public class OrderPlacedEventHandler : IHandlerAsync<OrderPlacedEvent>
    {
        private readonly IMessagePublisher _publisher;
        private readonly ILogger<OrderPlacedEventHandler> _logger;

        /// <summary>
        /// Handles messages of type OrderPlacedEvent
        /// Takes a dependency on IMessagePublisher so that further messages can be published 
        /// </summary>
        public OrderPlacedEventHandler(IMessagePublisher publisher, ILogger<OrderPlacedEventHandler> log)
        {
            _publisher = publisher;
            _logger = log;
        }

        public Task<bool> Handle(OrderPlacedEvent message)
        {
            _logger.LogInformation("Why am I telling myself that order {orderId} was placed!", message.OrderId);
            return Task.FromResult(true);
        }
    }
}
