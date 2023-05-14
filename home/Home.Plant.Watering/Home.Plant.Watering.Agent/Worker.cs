namespace Home.Plant.Watering.Agent;

using Grpc.Core;
using Grpc.Net.Client;
using System.Device.Gpio;

public class Worker : BackgroundService
{
    private const int Pin = 14;
    private bool IsOn { get; set; }
    
    private readonly ILogger<Worker> _logger;
    private readonly GrpcChannel _channel;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        var uri = configuration["ServerUri"] ?? throw new ArgumentException("Server Uri is not provided");
        _channel = GrpcChannel.ForAddress(uri);
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
                    await HandleRequest(reply.Seconds, stoppingToken);
                }
            }
            catch (RpcException)
            {
                _logger.LogInformation("Failed to connect to server.");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task HandleRequest(int seconds, CancellationToken cancellationToken)
    {
        using var controller = new GpioController();
        controller.OpenPin(Pin, PinMode.Output);
        controller.Write(Pin, ((IsOn) ? PinValue.High : PinValue.Low));
        await Task.Delay(seconds * 1000, cancellationToken);
        IsOn = !IsOn;
    }
    
    public override void Dispose()
    {
        _channel.Dispose();
        base.Dispose();
    }

}
