// Copyright 2016-2020, Pulumi Corporation

using System.Threading.Tasks;
using Grpc.Net.Client;
using Pulumirpc;

namespace Pulumi
{
    internal class GrpcEngine : IEngine
    {
        private readonly Engine.EngineClient _engine;
        private static GrpcChannel? _engineChannel = null;
        private readonly object _engineChannelLock = new object();

        public GrpcEngine(string engine)
        {
            // maxRpcMessageSize raises the gRPC Max Message size from `4194304` (4mb) to `419430400` (400mb)
            var maxRpcMessageSize = 400 * 1024 * 1024;
            lock (_engineChannelLock)
            {
                if (_engineChannel == null)
                {
                    // Inititialize the engine channel once
                    _engineChannel = GrpcChannel.ForAddress($"http://{engine}", new GrpcChannelOptions
                    {
                        MaxReceiveMessageSize = maxRpcMessageSize
                    });
                }
            }
            
            this._engine = new Engine.EngineClient(_engineChannel);
        }
        
        public async Task LogAsync(LogRequest request)
            => await this._engine.LogAsync(request);

        public async Task<SetRootResourceResponse> SetRootResourceAsync(SetRootResourceRequest request)
            => await this._engine.SetRootResourceAsync(request);

        public async Task<GetRootResourceResponse> GetRootResourceAsync(GetRootResourceRequest request)
            => await this._engine.GetRootResourceAsync(request);
    }
}
