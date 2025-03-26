using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Logic {
    [DataContract]
    public class ConsultPurchase {
        [DataMember]
        public string providerName { get; set; }
        [DataMember]
        public string providerContact { get; set; }
        [DataMember]
        public DateTime purchaseDate { get; set; }
        [DataMember]
        public string purchaseProducts { get; set; }
        [DataMember]
        public string payMethod { get; set; }
        [DataMember]
        public decimal amountPaid { get; set; }
    }
}
