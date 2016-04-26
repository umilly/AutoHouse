using System.Runtime.Serialization;

namespace ViewModel
{
    [DataContract]
    public class RelayData
    {
        [DataMember]
        public int Number { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string StartCommand { get; set; }
        [DataMember]
        public string StopCommand { get; set; }
    }
}