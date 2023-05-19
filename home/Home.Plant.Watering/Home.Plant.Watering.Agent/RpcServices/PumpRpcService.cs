namespace Home.Plant.Watering.Agent.RpcServices;

using Grpc.Core;
using Services;
using Shared;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

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
        
        await _pumpService.PumpStatusSubject.TakeWhile(async (pumpStatus, _) =>
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return false;
            }
            
            await responseStream.WriteAsync(new PumpStatusStreamResponse
            {
                Hostname = hostname, IsPumping = pumpStatus.IsPumping
            });

            return true;
        });
    }
}
