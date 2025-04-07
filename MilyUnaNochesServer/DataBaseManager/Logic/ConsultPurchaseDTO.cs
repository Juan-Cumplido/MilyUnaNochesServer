using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager.Logic {
    public class ConsultPurchaseDTO {
        public string providerName { get; set; }
        public string providerContact { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string purchasedProducts { get; set; }
        public string payMethod { get; set; }
        public decimal amountPaid { get; set; }
    }
}
