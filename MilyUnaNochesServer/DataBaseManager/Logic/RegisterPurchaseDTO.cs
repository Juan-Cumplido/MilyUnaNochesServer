using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager.Logic {
    public class RegisterPurchaseDTO {
            public int IdProveedor { get; set; }
            
            public string ContactoProveedor { get; set; }

            public DateTime Fecha { get; set; }

            public TimeSpan Hora { get; set; }

            public decimal MontoTotal { get; set; }

            public string PayMethod { get; set; }
            public List<ProductPurchaseDTO> Products { get; set; }
    }
}

