using System;
using System.Collections.Generic;
using System.Reflection;

namespace McBot.Gateway.Payloads
{
    internal class GatewayEvents
    {
        public string Value { get; set; }
        public Type Type { get; set; }

        public GatewayEvents()
        {
        }

        public GatewayEvents(string value)
        {
            Value = value;
        }

        public GatewayEvents(string value, Type type)
        {
            Value = value;
            Type = type;
        }

        public static GatewayEvents MessageCreated { get { return new GatewayEvents("MESSAGE_CREATE", typeof(MessageCreated)); } }
        public static GatewayEvents VoiceServerUpdate { get { return new GatewayEvents("VOICE_SERVER_UPDATE", typeof(VoiceServerUpdate)); } }
        public static GatewayEvents VoiceStateUpdate { get { return new GatewayEvents("VOICE_STATE_UPDATE", typeof(VoiceStateUpdate)); } }

        public static IList<GatewayEvents> GetGatewayEvents()
        {
            List<GatewayEvents> events = new List<GatewayEvents>();
            GatewayEvents evnt = new GatewayEvents();
            foreach (var property in typeof(GatewayEvents).GetProperties(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public))
            {
                var getMethod = property.GetGetMethod();

                var gatewayEvent = (GatewayEvents)getMethod.Invoke(evnt, null);
                if (gatewayEvent != null)
                {
                    events.Add(gatewayEvent);
                }
            }

            return events;

            /*  return new List<GatewayEvents>() {
                  MessageCreated,
                  VoiceServerUpdate,
                  VoiceStateUpdate,
              };*/
        }
    }
}