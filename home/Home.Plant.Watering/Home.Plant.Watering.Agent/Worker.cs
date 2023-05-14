namespace Home.Plant.Watering.Agent;

using System.Device.Gpio;

public class Worker : BackgroundService
{
    private const int Pin = 14;
    private bool IsOn { get; set; }
    
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);

            using var controller = new GpioController();
            controller.OpenPin(Pin, PinMode.Output);
            while (true)
            {
                controller.Write(Pin, ((IsOn) ? PinValue.High : PinValue.Low));
                await Task.Delay(3000, stoppingToken);
                IsOn = !IsOn;
            }
        }
    }
}
