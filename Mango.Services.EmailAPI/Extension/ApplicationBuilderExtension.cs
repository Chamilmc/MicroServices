﻿using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extension;

public static class ApplicationBuilderExtension
{
    private static IAzureServiceBusConsumer? azureServiceBusConsumer { get; set; }

    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        azureServiceBusConsumer = app!.ApplicationServices!.GetService<IAzureServiceBusConsumer>()!;
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
        hostApplicationLife!.ApplicationStarted.Register(OnStart);
        hostApplicationLife.ApplicationStopping.Register(OnStop);

        return app;
    }

    private static void OnStop()
    {
        azureServiceBusConsumer!.Stop();
    }

    private static void OnStart()
    {
        azureServiceBusConsumer!.Start();
    }
}
