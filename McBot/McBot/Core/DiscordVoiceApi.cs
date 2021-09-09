using McBot.Voice.Payloads;
using McBot.Voice.UdpPayloads;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class DiscordVoiceApi
    {
        private readonly SocketWrapper _wrapper;
        private readonly IOptions<AppSettings> _options;
        private readonly UdpClient _udpClient;

        public DiscordVoiceApi(
            SocketWrapper wrapper,
            IOptions<AppSettings> options,
            UdpClient udpClient
           )
        {
            _wrapper = wrapper;
            _options = options;
            _udpClient = udpClient;
        }

        public async Task IdentifyToVoice(VoicePayload payload)
        {
            try
            {
                var hello = await _wrapper.ConnectToSocket<VoicePayload>(((VoiceIdentify)payload.d).Endpoint);

                await _wrapper.SendPayload(payload);
                var readyPayload = await _wrapper.RecievePayload<VoicePayload>();

                var readyData = readyPayload.VoiceReady;

                var heartBeatPayload = new VoicePayload();
                heartBeatPayload.op = VoiceOpCodeEnumeration.Heartbeat;

                _ = _wrapper.SendHearthBeat(heartBeatPayload, (int)hello.VoiceHello.heartbeat_interval);

                ConnectToUdp(readyData);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public void ConnectToUdp(VoiceReady ready)
        {
            try
            {
                _udpClient.Connect(ready.Ip, ready.Port);
                IpDiscovery request = null;
                if (BitConverter.IsLittleEndian == true)
                {
                    request = new IpDiscovery
                    {
                        Type = ((short)0x1),
                        Length = ((short)70),
                        SSRC = BitConverter.ToUInt32(BitConverter.GetBytes((uint)ready.Ssrc).Reverse().ToArray(), 0),
                        Address = ready.Ip.GetAddressBytes(),
                        Port = BitConverter.ToUInt16(BitConverter.GetBytes((ushort)ready.Port).Reverse().ToArray(), 0)
                    };
                }

                var requestData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));

                var ServerEp = new IPEndPoint(IPAddress.Any, 0);
                _udpClient.Send(requestData, request.Length);
                if (_udpClient.Available > 0)
                {
                    var bytes = _udpClient.Receive(ref ServerEp);
                    IpDiscovery discovery = new IpDiscovery(bytes);
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}