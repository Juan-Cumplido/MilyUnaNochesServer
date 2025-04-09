using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Logic {
    [DataContract]    
    public class ProductPurchase {

    [DataMember]
    public int IdProducto { get; set; }

    [DataMember]
    public string ContactoProveedor { get; set; }
    [DataMember]
    public int Cantidad { get; set; }
    [DataMember]
    public decimal MontoProducto { get; set; }
    [DataMember]
    public string NombreProducto { get; set; }
    [DataMember]
    public string CodigoProducto { get; set; }
    }
}
