using McBot.Gateway.Payloads;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class DiscordVoiceApi
    {
        private readonly SocketWrapper _wrapper;
        private readonly IOptions<AppSettings> _options;

        public DiscordVoiceApi(SocketWrapper wrapper,
            IOptions<AppSettings> options
           )
        {
            _wrapper = wrapper;
            _options = options;
        }

        public async Task IdentifyToVoice(VoicePayload payload)
        {
            try
            {
                var hello = await _wrapper.ConnectToSocket<VoicePayload>(((VoiceIdentify)payload.d).Endpoint);

                await _wrapper.SendPayload(payload);
                var ready = await _wrapper.RecievePayload<VoicePayload>();

                var heartBeatPayload = new VoicePayload();
                heartBeatPayload.op = VoiceOpCodeEnumeration.Heartbeat;

                _ = _wrapper.SendHearthBeat(heartBeatPayload, (int)hello.VoiceHello.heartbeat_interval);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}