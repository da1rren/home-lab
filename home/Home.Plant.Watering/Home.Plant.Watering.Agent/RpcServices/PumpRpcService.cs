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
        await _pumpService.StartPump(context.CancellationToken);
        return new StartPumpResponse
        {
            
        };
    }

    public override async Task<StopPumpResponse> Stop(StopPumpRequest request, ServerCallContext context)
    {
        await _pumpService.StopPump(context.CancellationToken);
        return new StopPumpResponse { };
    }

    public override async Task StatusStream(PumpStatusStreamRequest request, IServerStreamWriter<PumpStatusStreamResponse> responseStream, ServerCallContext context)
    {
        var hostname = Environment.MachineName;

        var subscription = await _pumpService.PumpStatusSubject.SubscribeAsync(async pumpStatus =>
        {
            await responseStream.WriteAsync(new PumpStatusStreamResponse
            {
                Hostname = hostname, 
                IsPumping = pumpStatus.IsPumping
            });
        });

        await subscription.DisposeAsync();
    }
}
