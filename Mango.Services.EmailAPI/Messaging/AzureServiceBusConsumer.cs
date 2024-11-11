using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _emailCartQueue;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private ServiceBusProcessor _emailCartProcessor;

    public AzureServiceBusConsumer(IConfiguration configuration,
        EmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;
        _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnnectionString")!;
        _emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShopingCartQueue")!;
        var client = new ServiceBusClient(_serviceBusConnectionString);
        _emailCartProcessor = client.CreateProcessor(_emailCartQueue);
    }

    public async Task Start()
    {
        _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
        _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
        await _emailCartProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await _emailCartProcessor.StopProcessingAsync();
        await _emailCartProcessor.DisposeAsync();
    }

    private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
    {
        // This is where you will get message
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body)!;

        try
        {
            // To DO
            await _emailService!.EmailCartAndLog(objMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}
