using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Logic {
    [DataContract]
    public class RegisterPurchase_sv {
        [DataMember]
        public int IdProveedor { get; set; }
        [DataMember]
        public string ContactoProveedor { get; set; }
        [DataMember]
        public DateTime Fecha { get; set; }
        [DataMember]
        public TimeSpan Hora { get; set; }
        [DataMember]
        public decimal MontoTotal { get; set; }
        [DataMember]
        public string PayMethod { get; set; }
        [DataMember]
        public List<ProductPurchase> Products { get; set; }
    }
}
