using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Logic {
    [DataContract]
    public class ConsultPurchase_SV {
        [DataMember]
        public string providerName { get; set; }
        [DataMember]
        public string providerContact { get; set; }
        public DateTime Fecha { get; set; }
        [DataMember]
        public TimeSpan Hora { get; set; }
        [DataMember]
        public string purchasedProducts { get; set; }
        [DataMember]
        public string payMethod { get; set; }
        [DataMember]
        public decimal amountPaid { get; set; }
    }
}
