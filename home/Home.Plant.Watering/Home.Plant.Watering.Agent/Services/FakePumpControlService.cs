namespace Home.Plant.Watering.Agent.Services;

public class FakePumpControlService : IPumpControlService
{
    public event EventHandler<StatusChangedEvent>? StatusChanged;
    
    private readonly ILogger<FakePumpControlService> _logger;
    private bool _isPumping;
    
    public FakePumpControlService(ILogger<FakePumpControlService> logger)
    {
        _logger = logger;
    }
    
    public bool IsPumping()
    {
        _logger.LogInformation($"Is Pumping: {_isPumping}");
        return _isPumping;
    }

    public void StartPump()
    {
        _logger.LogInformation($"Started Pump");
        StatusChanged?.Invoke(this, new StatusChangedEvent(true));
        _isPumping = true;
    }

    public void StopPump()
    {
        _logger.LogInformation($"Stopped Pump");
        StatusChanged?.Invoke(this, new StatusChangedEvent(false));
        _isPumping = false;
    }

    public void Dispose()
    {
        
    }
}
