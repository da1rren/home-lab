namespace Home.Plant.Watering.Agent.RpcServices;

using Grpc.Core;
using Services;
using Shared;

public class PumpRpcService : PumpService.PumpServiceBase
{
    private readonly IPumpControlService _pumpService;

    public PumpRpcService(IPumpControlService pumpService)
    {
        _pumpService = pumpService;
    }
    
    public override async Task<StartPumpResponse> Start(StartPumpRequest request, ServerCallContext context)
    {
        _pumpService.StartPump();
        return new StartPumpResponse
        {
            
        };
    }

    public override async Task<StopPumpResponse> Stop(StopPumpRequest request, ServerCallContext context)
    {
        _pumpService.StopPump();
        return new StopPumpResponse { };
    }

    public override async Task StatusStream(PumpStatusStreamRequest request, IServerStreamWriter<PumpStatusStreamResponse> responseStream, ServerCallContext context)
    {
        var hostname = Environment.MachineName;

        async void OnPumpServiceOnStatusChanged(object? _, StatusChangedEvent args)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            
            await responseStream.WriteAsync(new PumpStatusStreamResponse
            {
                Hostname = hostname, 
                IsPumping = args.IsPumping
            });
        }
        
        await responseStream.WriteAsync(new PumpStatusStreamResponse
        {
            Hostname = hostname, IsPumping = _pumpService.IsPumping()
        });
        
        _pumpService.StatusChanged += OnPumpServiceOnStatusChanged;

        try
        {
            await Task.Delay(int.MaxValue, context.CancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Ignore
        }

        // Need to unregister and close out 
        _pumpService.StatusChanged -= OnPumpServiceOnStatusChanged;
    }
}
