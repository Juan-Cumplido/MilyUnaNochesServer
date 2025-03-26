using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Logic {
    [DataContract]
    public class RegisterPurchase {
        [DataMember]
        public int IdProveedor { get; set; }
        [DataMember]
        public DateTime Fecha { get; set; }
        [DataMember]
        public TimeSpan Hora { get; set; }
        [DataMember]
        public decimal MontoTotal { get; set; }
        [DataMember]
        public string PayMethod { get; set; }
        [DataMember]
        public List<PurchaseProductDTO> Products { get; set; }
    }

    [DataContract]
    public class PurchaseProductDTO {
        [DataMember]
        public int IdProducto { get; set; }
        [DataMember]
        public int Cantidad { get; set; }
        [DataMember]
        public decimal MontoProducto { get; set; }
    }
}
