using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ViewModel
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public List<RelayData> Relays { get; set; }
        [DataMember]
        public long Count { get; set; }
        [DataMember]
        public bool IsDebug { get; set; }
    }
}