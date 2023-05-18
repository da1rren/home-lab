namespace Home.Plant.Watering.Agent.Services;

using System.Device.Gpio;

public record StatusChangedEvent(bool IsPumping);

public class PumpControlService : IPumpControlService
{
    private static readonly object _lock = new();
    private readonly GpioController _controller;
    private const int Pin = 14;

    public event EventHandler<StatusChangedEvent>? StatusChanged;
    
    public PumpControlService()
    {
        _controller = new GpioController();
    }

    public bool IsPumping()
    {
        lock (_lock)
        {
            _controller.OpenPin(Pin);
            var pinValue = _controller.Read(Pin);
            _controller.ClosePin(Pin);
            return pinValue != PinValue.High;
        }
    }

    public void StartPump()
    {
        lock (_lock)
        {
            _controller.OpenPin(Pin, PinMode.Output);
            _controller.Write(Pin, PinValue.Low);
            _controller.ClosePin(Pin);
        }
        
        StatusChanged?.Invoke(this, new StatusChangedEvent(true));
    }

    public void StopPump()
    {
        lock (_lock)
        {
            _controller.OpenPin(Pin, PinMode.Output);
            _controller.Write(Pin, PinValue.High);
            _controller.ClosePin(Pin);
        }
        
        StatusChanged?.Invoke(this, new StatusChangedEvent(false));
    }

    public void Dispose()
    {
        _controller.Dispose();
    }
}
