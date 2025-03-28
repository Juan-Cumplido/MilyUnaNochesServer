using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager.Logic {
    public class ProductPurchaseDTO {

        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal MontoProducto { get; set; }
    }
}
