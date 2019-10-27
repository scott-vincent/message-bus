using System;
using System.Threading.Tasks;
using KitchenOrders.KitchenConsole.Handlers;
using KitchenOrders.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KitchenOrders.KitchenConsole
{
    class Program
    {
        public static async Task Main()
        {
            Console.Title = "KitchenConsole";

            await new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false);
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                })
               .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
               .ConfigureServices((hostContext, services) =>
               {
                   var configuration = hostContext.Configuration;
                   services.AddJustSaying(config =>
                   {
                       config.Client(x =>
                       {
                           if (configuration.HasAWSServiceUrl())
                           {
                               // The AWS client SDK allows specifying a custom HTTP endpoint.
                               // For testing purposes it is useful to specify a value that
                               // points to a docker image such as `p4tin/goaws` or `localstack/localstack`
                               x.WithServiceUri(configuration.GetAWSServiceUri())
                                .WithAnonymousCredentials();
                           }
                           else
                           {
                               // The real AWS environment will require some means of authentication
                               //x.WithBasicCredentials("###", "###");
                               //x.WithSessionCredentials("###", "###", "###");
                           }
                       });

                       config.Messaging(x =>
                       {
                           // Configures which AWS Region to operate in
                           x.WithRegion(configuration.GetAWSRegion());
                       });

                       config.Subscriptions(x =>
                       {
                           // Creates the following if they do not already exist
                           //  - a SQS queue of name `kitchenconsole-orderplacedevent`
                           //  - a SQS queue of name `kitchenconsole-orderplacedevent_error`
                           //  - a SNS topic of name `orderplacedevent`
                           //  - a SNS topic subscription on topic 'orderplacedevent'
                           x.ForTopic<OrderPlacedEvent>(QueueName.Create<OrderPlacedEvent>(configuration));

                           // Add another subscription just to show that we can (subscribing to our own publish!)
                           x.ForTopic<OrderReadyEvent>(QueueName.Create<OrderReadyEvent>(configuration));
                       });

                       config.Publications(x =>
                       {
                           // Creates the following if they do not already exist
                           //  - a SNS topic of name `orderreadyevent`
                           x.WithTopic<OrderReadyEvent>();
                       });
                   });

                   // Added a message handler for message type for 'OrderPlacedEvent' on topic 'orderplacedevent'
                   services.AddJustSayingHandler<OrderPlacedEvent, OrderPlacedEventHandler>();

                   // Added another message handler for message type for 'OrderReadyEvent' on topic 'orderreadyevent'
                   services.AddJustSayingHandler<OrderReadyEvent, OrderReadyEventHandler>();

                   // Add a background service that is listening for messages related to the above subscriptions
                   services.AddHostedService<Subscriber>();
               })
              .UseConsoleLifetime()
              .Build()
              .RunAsync();
        }
    }
}
