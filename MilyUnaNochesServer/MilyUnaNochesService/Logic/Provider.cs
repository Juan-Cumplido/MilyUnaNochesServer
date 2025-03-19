using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Logic {
    [DataContract]
    public class Provider {
        [DataMember]
        public int IdProvider { get; set; }
        [DataMember]
        public int idAddress { get; set; }
        [DataMember]
        public string providerName { get; set; }
        [DataMember]
        public string providerContact {  get; set; }
        [DataMember]
        public string phoneNumber { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public string providerAddress { get; set; }
    }
}
