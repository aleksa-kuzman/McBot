using McBot.Extensions;
using McBot.Voice.Payloads;
using McBot.Voice.UdpPayloads;
using Microsoft.Extensions.Options;
using NaCl;
using NAudio.Wave;
using OpusDotNet;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Speech.Synthesis;
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
        private readonly Random _random;

        public DiscordVoiceApi(
            SocketWrapper wrapper,
            IOptions<AppSettings> options,
            UdpClient udpClient,
            Random random
           )
        {
            _wrapper = wrapper;
            _options = options;
            _udpClient = udpClient;
            _random = random;
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

                var speaking = new VoicePayload();
                speaking.op = VoiceOpCodeEnumeration.Speaking;

                var speakingData = new Speaking()
                {
                    SpeakingFlag = 5,
                    Delay = 0,
                    Ssrc = readyData.Ssrc
                };
                speaking.d = speakingData;
                await _wrapper.SendPayload(speaking);

                LoadVoiceFile("Test2.wav", readyData.Ssrc, sessionDescription.SecretKey);

                ((Speaking)speaking.d).SpeakingFlag = 0;
                await _wrapper.SendPayload(speaking);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public static Stream Speak(string text)
        {
            SpeechSynthesizer s = new SpeechSynthesizer();
            MemoryStream stream = new MemoryStream();
            s.SetOutputToWaveStream(stream);
            s.Speak(text);
            s.SetOutputToNull();
            return stream;
        }

        private WaveStream ConvertToPcmStream(WaveFileReader reader)
        {
            WaveFormat newFormat = new WaveFormat(48000, reader.WaveFormat.Channels);
            WaveFormatConversionStream newStream = new WaveFormatConversionStream(newFormat, reader);
            WaveStream converted = WaveFormatConversionStream.CreatePcmStream(newStream);

            return converted;

            //byte[] bytes = new byte[converted.Length];
            //converted.Position = 0;
            //converted.Read(bytes, 0, (int)converted.Length);
            //return bytes;
        }

        //public float[] ConvertToFloatArray(WaveStream stream)
        //{
        //        byte[] buffer = new byte[2]
        //        while (stream.Position < stream.Length)
        //        {
        //            float[] data = stream.Read(buffer,0,buffer.le)
        //            if (data == null)
        //                break;

        //            length += data.Length;

        //            buffers.Add(data);
        //        }

        //    return buffers;
        //}

        internal short[] BytesToShorts(byte[] input, int offset, int length)
        {
            short[] processedValues = new short[length / 2];
            for (int c = 0; c < processedValues.Length; c++)
            {
                processedValues[c] = (short)(((int)input[(c * 2) + offset]) << 0);
                processedValues[c] += (short)(((int)input[(c * 2) + 1 + offset]) << 8);
            }

            return processedValues;
        }

        private byte[] CreateNonce(VoicePacket voicePacket)
        {
            byte[] nonce = new byte[24];

            var arraySsrc = BitConverter.GetBytes(voicePacket.SSRC);
            var arrayTimestamp = BitConverter.GetBytes(voicePacket.Timestamp);
            var arraySeq = BitConverter.GetBytes(voicePacket.Sequence);

            BinaryFormatter bf = new BinaryFormatter();

            nonce[0] = voicePacket.VersionFlags;
            nonce[1] = voicePacket.PayloadType;

            Buffer.BlockCopy(arraySeq, 0, nonce, 2, arraySeq.Length);
            Buffer.BlockCopy(arrayTimestamp, 0, nonce, 4, arrayTimestamp.Length);
            Buffer.BlockCopy(arraySsrc, 0, nonce, 8, arraySsrc.Length);

            return nonce;
        }

        public void LoadVoiceFile(string filename, int ssrc, byte[] key)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, filename);

            WaveFileReader reader = new WaveFileReader(path);
            var pcmStream = ConvertToPcmStream(reader);
            //var chunkedPcm = ConvertToChunks(pcmStream);

            /////
            int size = 0;

            //// 48000 / 1000 * 2(mikrosekunde) * 2((bajta) znaci 192
            byte[] pcmChunk = new byte[pcmStream.Length];
            pcmStream.Position = 0;
            size = pcmStream.Read(pcmChunk, 0, (int)pcmStream.Length);

            var shortArray = BytesToShorts(pcmChunk, 0, pcmChunk.Length);

            OpusEncoder encoder = new OpusEncoder(Application.VoIP, 48000, 2);
            var a = encoder.Bitrate;

            // 48000 / 1000 * 20  * 2 kanala znaci 960
            int frameSize = 960 * 2;

            byte[] outputBuffer = new byte[3000];

            var startingSequence = (ushort)_random.Next(0, 1233);
            var timestamp = (ushort)_random.Next(0, 1233);

            int thisPacketSize = 0;
            //
            //
            //

            for (int i = 0; i < shortArray.Length - frameSize; i = i + frameSize)
            {
                var currentChunk = pcmChunk.Skip(i).Take(frameSize).ToArray();
                thisPacketSize = encoder.Encode(currentChunk, currentChunk.Length, outputBuffer, outputBuffer.Length);

                VoicePacket voicePacket = new VoicePacket();
                voicePacket.Sequence = startingSequence.ConvertToBigEndian();
                voicePacket.SSRC = ((uint)ssrc).ConvertToBigEndian();
                voicePacket.Timestamp = timestamp;

                byte[] nonce = CreateNonce(voicePacket);

                var cipher = new byte[outputBuffer.Length + XSalsa20Poly1305.TagLength];
                XSalsa20Poly1305 xSalsa20Poly1305 = new XSalsa20Poly1305(key);

                xSalsa20Poly1305.Encrypt(cipher, outputBuffer, nonce);
                _udpClient.Send(cipher, cipher.Length);

                Array.Clear(outputBuffer, 0, outputBuffer.Length);
                Array.Clear(currentChunk, 0, currentChunk.Length);
                startingSequence++;
                timestamp += 960;
            }

            reader.Dispose();

            Console.WriteLine("asdf");
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
                        SSRC = ((uint)ready.Ssrc).ConvertToBigEndian(),// BitConverter.ToUInt32(BitConverter.GetBytes().Reverse().ToArray(), 0),
                        Address = ready.Ip.GetAddressBytes(),
                        Port = ((ushort)ready.Port).ConvertToBigEndian()
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