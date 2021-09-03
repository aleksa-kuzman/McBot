using McBot.Core;

namespace McBot.Gateway.Payloads
{
    public class VoiceOpCodeEnumeration : EnumerationBase
    {
        public static VoiceOpCodeEnumeration Identify = new VoiceOpCodeEnumeration(0, nameof(Identify).ToLower());
        public static VoiceOpCodeEnumeration SelectProtocol = new VoiceOpCodeEnumeration(1, nameof(SelectProtocol).ToLower());
        public static VoiceOpCodeEnumeration Ready = new VoiceOpCodeEnumeration(2, nameof(Ready).ToLower());
        public static VoiceOpCodeEnumeration Heartbeat = new VoiceOpCodeEnumeration(3, nameof(Heartbeat).ToLower());
        public static VoiceOpCodeEnumeration SessionDescription = new VoiceOpCodeEnumeration(4, nameof(SessionDescription).ToLower());
        public static VoiceOpCodeEnumeration Speaking = new VoiceOpCodeEnumeration(5, nameof(Speaking).ToLower());
        public static VoiceOpCodeEnumeration HeartbeatACK = new VoiceOpCodeEnumeration(6, nameof(HeartbeatACK).ToLower());
        public static VoiceOpCodeEnumeration Resume = new VoiceOpCodeEnumeration(7, nameof(Resume).ToLower());
        public static VoiceOpCodeEnumeration Hello = new VoiceOpCodeEnumeration(8, nameof(Hello).ToLower());
        public static VoiceOpCodeEnumeration Resumed = new VoiceOpCodeEnumeration(9, nameof(Resumed).ToLower());
        public static VoiceOpCodeEnumeration ClientDisconnect = new VoiceOpCodeEnumeration(13, nameof(ClientDisconnect).ToLower());

        public VoiceOpCodeEnumeration(int Id, string name) : base(name, Id)
        {
        }
    }
}