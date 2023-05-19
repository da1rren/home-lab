namespace Home.Plant.Watering.Agent.Services;

using System.Reactive.Subjects;

public class FakePumpControlService : IPumpControlService
{
    public SequentialBehaviorAsyncSubject<PumpStatus> PumpStatusSubject { get; }
    
    private readonly ILogger<FakePumpControlService> _logger;
    private bool _isPumping;

    public FakePumpControlService(ILogger<FakePumpControlService> logger)
    {
        _logger = logger;
        PumpStatusSubject = new SequentialBehaviorAsyncSubject<PumpStatus>(
            new PumpStatus(false, null));
    }

    public Task<bool> IsPumping(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Is Pumping: {_isPumping}");
        return Task.FromResult(_isPumping);
    }

    public async Task StartPump(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Started Pump");
        _isPumping = true;
        await PumpStatusSubject.OnNextAsync(new PumpStatus(_isPumping, DateTimeOffset.Now));
    }

    public async Task StopPump(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Stopped Pump");
        _isPumping = false;
        await PumpStatusSubject.OnNextAsync(new PumpStatus(_isPumping, DateTimeOffset.Now));
    }
    
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
