#pragma warning disable CS1998
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

    public async Task<bool> IsPumping(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Is Pumping: {_isPumping}");
        return _isPumping;
    }

    public async Task StartPump(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Started Pump");
        _isPumping = true;
    }

    public async Task StopPump(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Stopped Pump");
        _isPumping = false;
    }
    
    public async ValueTask DisposeAsync()
    {
        // Nothing
    }
}
