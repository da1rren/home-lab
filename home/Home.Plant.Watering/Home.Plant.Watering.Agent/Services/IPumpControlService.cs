namespace Home.Plant.Watering.Agent.Services;

public interface IPumpControlService : IDisposable
{
    event EventHandler<StatusChangedEvent>? StatusChanged;
    
    bool IsPumping();
    
    void StartPump();
    
    void StopPump();
}
