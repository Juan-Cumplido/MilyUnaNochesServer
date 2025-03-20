using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MilyUnaNochesService.Logic
{
    [DataContract]
    public class Product
    {
        [DataMember]
        public int IdProducto { get; set; }
        [DataMember]
        public string NombreProducto { get; set; }
        [DataMember]
        public string CodigoProducto { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public decimal PrecioCompra { get; set; }
        [DataMember]
        public decimal PrecioVenta { get; set; }
        [DataMember]
        public string Categoria { get; set; }
        [DataMember]
        public int Cantidad { get; set; }
        [DataMember]
        public byte[] Imagen { get; set; }
    }
}
