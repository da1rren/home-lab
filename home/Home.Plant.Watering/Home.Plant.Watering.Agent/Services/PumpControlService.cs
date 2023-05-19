namespace Home.Plant.Watering.Agent.Services;

using System.Device.Gpio;
using System.Reactive.Subjects;

public record PumpStatus(bool IsPumping, DateTimeOffset? LastActivatedAt);

public class PumpControlService : IPumpControlService
{
    private readonly GpioController _controller;
    private readonly SemaphoreSlim _semaphore;
    private const int Pin = 14;

    public SequentialBehaviorAsyncSubject<PumpStatus> PumpStatusSubject { get; }
    
    public PumpControlService()
    {
        _controller = new GpioController();
        _semaphore = new SemaphoreSlim(1);
        PumpStatusSubject = new SequentialBehaviorAsyncSubject<PumpStatus>(
            new PumpStatus(false, null));
    }
    public async Task<bool> IsPumping(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        
        try
        {
            _controller.OpenPin(Pin);
            var pinValue = _controller.Read(Pin);
            _controller.ClosePin(Pin);

            var isPumping = pinValue != PinValue.High;

            await PumpStatusSubject.OnNextAsync(PumpStatusSubject.Value with
            {
                IsPumping = isPumping
            });
            
            return isPumping;
        }
        finally
        {
            _semaphore.Release();
        }
        
    }

    public async Task StartPump(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        
        try
        {
            _controller.OpenPin(Pin, PinMode.Output);
            _controller.Write(Pin, PinValue.Low);
            _controller.ClosePin(Pin);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task StopPump(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        
        try
        {
            _controller.OpenPin(Pin, PinMode.Output);
            _controller.Write(Pin, PinValue.High);
            _controller.ClosePin(Pin);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        await PumpStatusSubject.OnCompletedAsync();
        _controller.Dispose();
    }
}
