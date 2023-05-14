namespace Home.Plant.Watering.Ui.RpcServices;

using Grpc.Core;

public class PumpRpcService : PumpService.PumpServiceBase
{
    public override async Task Start(PumpStartRequest request, IServerStreamWriter<PumpStartResponse> responseStream, 
        ServerCallContext context)
    {
        await responseStream.WriteAsync(new PumpStartResponse {Seconds = 5});
    }
}
