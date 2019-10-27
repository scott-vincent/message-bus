using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace KitchenOrders.Messages
{
    public static class QueueName
    {
        // Each service must have its name defined in appsettings.json
        private const string ServiceNameKey = "ServiceName";

        /// <summary>
        /// We want subscriber queue names to be in the following format:
        /// 
        ///     <serviceName>-<messageType>
        /// 
        /// </summary>
        public static string Create<T>(IConfiguration configuration)
        {
            var serviceName = configuration[ServiceNameKey];

            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new Exception($"{ServiceNameKey} must be defined in appsettings.json");
            }

            return serviceName + "-" + typeof(T).Name;
        }
    }
}
