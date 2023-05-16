namespace Home.Plant.Watering.Ui.RpcServices;

using Grpc.Core;
using System.Collections.Concurrent;

public class PumpRpcService : PumpService.PumpServiceBase
{
    public static readonly BlockingCollection<bool> PumpRequests = new();
    
    public override async Task Start(PumpStartRequest request, IServerStreamWriter<PumpStartResponse> responseStream, 
        ServerCallContext context)
    {
        while (PumpRequests.TryTake(out var shouldPump, int.MaxValue, context.CancellationToken))
        {
            await responseStream.WriteAsync(new PumpStartResponse {ShouldPump = shouldPump});
        }
    }
}
