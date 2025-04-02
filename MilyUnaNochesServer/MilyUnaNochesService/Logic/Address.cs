using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Logic {
    [DataContract]
    public class Address {
        [DataMember]
        public int IdDireccion { get; set; }
        [DataMember]
        public string Calle { get; set; }
        [DataMember]
        public string Numero { get; set; }
        [DataMember]
        public string CodigoPostal { get; set; }
        [DataMember]
        public string Ciudad { get; set; }
    }
}
