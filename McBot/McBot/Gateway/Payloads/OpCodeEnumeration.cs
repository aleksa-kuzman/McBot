using McBot.Core;

namespace McBot.Gateway.Payloads
{
    public class OpCodeEnumeration : EnumerationBase
    {
        public static OpCodeEnumeration Ready = new OpCodeEnumeration(0, nameof(Ready).ToLower());
        public static OpCodeEnumeration HeartBeat = new OpCodeEnumeration(1, nameof(HeartBeat).ToLower());
        public static OpCodeEnumeration Identify = new OpCodeEnumeration(2, nameof(Identify).ToLower());
        public static OpCodeEnumeration VoiceStateUpdate = new OpCodeEnumeration(4, nameof(VoiceStateUpdate).ToLower());
        public static OpCodeEnumeration Resume = new OpCodeEnumeration(6, nameof(Resume).ToLower());
        public static OpCodeEnumeration InvalidSession = new OpCodeEnumeration(9, nameof(InvalidSession).ToLower());
        public static OpCodeEnumeration Hello = new OpCodeEnumeration(10, nameof(Hello).ToLower());
        public static OpCodeEnumeration Ack = new OpCodeEnumeration(11, nameof(Ack).ToLower());

        public OpCodeEnumeration(int Id, string name) : base(name, Id)
        {
        }
    }
}