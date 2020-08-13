using System.Collections.Generic;

namespace MC_Server_Starter.Gateway
{
    internal class GatewayResource
    {
        public string Url { get; set; }
        public int Shards { get; set; }
        public Dictionary<string, int> Session_Start_Limit { get; set; }
    }
}