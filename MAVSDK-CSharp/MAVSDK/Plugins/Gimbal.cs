using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Mavsdk.Rpc.Gimbal;

using Version = Mavsdk.Rpc.Info.Version;

namespace MAVSDK.Plugins
{
    public class Gimbal
    {
        private readonly GimbalService.GimbalServiceClient _gimbalServiceClient;

        internal Gimbal(Channel channel)
        {
            _gimbalServiceClient = new GimbalService.GimbalServiceClient(channel);
        }

        public IObservable<Unit> SetPitchAndYaw(float pitchDeg, float yawDeg)
        {
            return Observable.Create<Unit>(observer =>
            {
                var request = new SetPitchAndYawRequest();
                request.PitchDeg = pitchDeg;
                request.YawDeg = yawDeg;
                var setPitchAndYawResponse = _gimbalServiceClient.SetPitchAndYaw(request);
                var gimbalResult = setPitchAndYawResponse.GimbalResult;
                if (gimbalResult.Result == GimbalResult.Types.Result.Success)
                {
                    observer.OnCompleted();
                }
                else
                {
                    observer.OnError(new GimbalException(gimbalResult.Result, gimbalResult.ResultStr));
                }

                return Task.FromResult(Disposable.Empty);
            });
        }
    }

    public class GimbalException : Exception
    {
        public GimbalResult.Types.Result Result { get; }
        public string ResultStr { get; }

        public GimbalException(GimbalResult.Types.Result result, string resultStr)
        {
            Result = result;
            ResultStr = resultStr;
        }
    }
}