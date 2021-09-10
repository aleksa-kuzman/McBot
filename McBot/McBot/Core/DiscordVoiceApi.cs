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

                var ipPortDiscovery = await ConnectToUdp(readyData);

                var sessionDescription = await SelectProtocol(ipPortDiscovery);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public async Task<SessionDescription> SelectProtocol(IpDiscovery ipPortDiscovery)
        {
            SelectPayloadProtocol selectPayload = new SelectPayloadProtocol()
            {
                Protocol = "udp",
                Data = new SelectPayloadProtocolData
                {
                    Address = Encoding.UTF8.GetString(ipPortDiscovery.Address),
                    Port = ipPortDiscovery.Port,
                    Mode = "xsalsa20_poly1305_lite"
                }
            };

            VoicePayload payload = new VoicePayload();

            payload.op = VoiceOpCodeEnumeration.SelectProtocol;
            payload.d = selectPayload;

            await _wrapper.SendPayload<VoicePayload>(payload);
            var sessionDescription = await _wrapper.RecievePayload<VoicePayload>();

            return sessionDescription.SessionDescription;
        }

        public async Task<IpDiscovery> ConnectToUdp(VoiceReady ready)
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

                var serverEp = new IPEndPoint(IPAddress.Any, 0);
                await _udpClient.SendAsync(requestData, request.Length);

                Console.WriteLine("Available on client: " + _udpClient.Available);
                while (_udpClient.Available == 0)
                {
                }
                var bytes = _udpClient.Receive(ref serverEp);
                IpDiscovery discovery = new IpDiscovery(bytes);

                return discovery;

                throw new Exception("Ip not discovered");
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}