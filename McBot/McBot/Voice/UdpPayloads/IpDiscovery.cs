using System;

namespace McBot.Voice.UdpPayloads
{
    public class IpDiscovery
    {
        public short Type { get; set; }
        public short Length { get; set; }
        public uint SSRC { get; set; }
        public byte[] Address { get; set; }
        public ushort Port { get; set; }

        public IpDiscovery()
        {
        }

        public IpDiscovery(short type, short length, uint sSRC, byte[] address, ushort port)
        {
            Type = type;
            Length = length;
            SSRC = sSRC;
            Address = address;
            Port = port;
        }

        public IpDiscovery(byte[] bytes)
        {
            if (bytes.Length != 70)
            {
                throw new System.Exception();
            }
            var ssrc = bytes[0..4];
            Array.Reverse(ssrc);

            var port = bytes[68..70];
            Array.Reverse(port);

            Address = bytes[4..68];
            SSRC = BitConverter.ToUInt32(ssrc);
            Port = BitConverter.ToUInt16(port);
        }
    }
}