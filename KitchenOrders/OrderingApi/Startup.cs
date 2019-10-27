using KitchenOrders.Messages;
using KitchenOrders.OrderingApi.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace KitchenOrders.OrderingApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddJustSaying(config =>
            {
                config.Client(x =>
                {
                    if (_configuration.HasAWSServiceUrl())
                    {
                        // The AWS client SDK allows specifying a custom HTTP endpoint.
                        // For testing purposes it is useful to specify a value that
                        // points to a docker image such as `p4tin/goaws` or `localstack/localstack`
                        x.WithServiceUri(_configuration.GetAWSServiceUri())
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
                    x.WithRegion(_configuration.GetAWSRegion());
                });
                config.Subscriptions(x =>
                {
                    // Creates the following if they do not already exist
                    //  - a SQS queue of name `orderingapi-orderreadyevent`
                    //  - a SQS queue of name `orderingapi-orderreadyevent_error`
                    //  - a SNS topic of name `orderreadyevent`
                    //  - a SNS topic subscription on topic 'orderreadyevent'
                    x.ForTopic<OrderReadyEvent>(QueueName.Create<OrderReadyEvent>(_configuration));

                    // Add another subscription just to show that we can (subscribing to our own publish!)
                    x.ForTopic<OrderPlacedEvent>(QueueName.Create<OrderPlacedEvent>(_configuration));
                });
                config.Publications(x =>
                {
                    // Creates the following if they do not already exist
                    //  - a SNS topic of name `orderplacedevent`
                    x.WithTopic<OrderPlacedEvent>();
                });
            });

            // Added a message handler for message type for 'OrderReadyEvent' on topic 'orderreadyevent'
            services.AddJustSayingHandler<OrderReadyEvent, OrderReadyEventHandler>();

            // Added another message handler for message type for 'OrderPlacedEvent' on topic 'orderplacedevent'
            services.AddJustSayingHandler<OrderPlacedEvent, OrderPlacedEventHandler>();

            // Add a background service that is listening for messages related to the above subscriptions
            services.AddHostedService<Subscriber>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant Ordering API", Version = "v1" });
            });
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseEndpoints((endpoints) => endpoints.MapDefaultControllerRoute());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Ordering API");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
