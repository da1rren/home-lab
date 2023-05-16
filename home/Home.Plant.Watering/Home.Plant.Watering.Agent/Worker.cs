namespace Home.Plant.Watering.Agent;

using Grpc.Core;
using Grpc.Net.Client;
using System.Device.Gpio;

public class Worker : BackgroundService
{
    private const int Pin = 14;
    
    private PinValue _pinStatus;
    private readonly ILogger<Worker> _logger;
    private readonly GrpcChannel _channel;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        var uri = configuration["ServerUri"] ?? throw new ArgumentException("Server Uri is not provided");
        
        // Disable tls check for the moment....
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var httpClient = new HttpClient(httpClientHandler);
        
        _channel = GrpcChannel.ForAddress(uri, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

        var pumpService = new PumpService.PumpServiceClient(_channel);
        
        var request = new PumpStartRequest
        {
            Hostname = Environment.MachineName
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var stream = pumpService.Start(request, cancellationToken: stoppingToken)
                .ResponseStream.ReadAllAsync(cancellationToken: stoppingToken);

            try
            {
                await foreach (var reply in stream.WithCancellation(stoppingToken))
                {
                    await HandleRequest(reply.ShouldPump, stoppingToken);
                }
            }
            catch (RpcException ex)
            {
                _logger.LogInformation(ex, "Failed to connect to server.");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task HandleRequest(bool shouldPump, CancellationToken cancellationToken)
    {
        var desiredPinValue = shouldPump ? PinValue.High : PinValue.Low;

        // Nothing to change
        if (desiredPinValue == _pinStatus)
        {
            return;
        }
        
        using var controller = new GpioController();
        controller.OpenPin(Pin, PinMode.Output);
        controller.Write(Pin, _pinStatus);
    }
    
    public override void Dispose()
    {
        _channel.Dispose();
        base.Dispose();
    }

}
