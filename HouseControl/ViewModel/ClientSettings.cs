using System.Runtime.Serialization;

namespace ViewModel
{
    [DataContract]
    public class ClientSettings
    {
        [DataMember]
        public string ServerIP { get; set; }
        [DataMember]
        public int ServerPort { get; set; }
    }
}