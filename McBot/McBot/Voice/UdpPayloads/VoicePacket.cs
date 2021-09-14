using System;

namespace McBot.Voice.UdpPayloads
{
    [Serializable]
    public class VoicePacket
    {
        public byte VersionFlags { get; set; }

        public byte PayloadType { get; set; }

        public ushort Sequence { get; set; }

        public uint Timestamp { get; set; }

        public uint SSRC { get; set; }

        public byte[] Data { get; set; }

        public VoicePacket()
        {
            VersionFlags = 0x80;
            PayloadType = 0x78;
        }
    }
}