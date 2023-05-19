namespace Home.Plant.Watering.Agent.Services;

using System.Reactive.Subjects;

public interface IPumpControlService : IAsyncDisposable
{
    SequentialBehaviorAsyncSubject<PumpStatus> PumpStatusSubject { get; }
    
    Task<bool> IsPumping(CancellationToken cancellationToken);
    
    Task StartPump(CancellationToken cancellationToken);
    
    Task StopPump(CancellationToken cancellationToken);
}
