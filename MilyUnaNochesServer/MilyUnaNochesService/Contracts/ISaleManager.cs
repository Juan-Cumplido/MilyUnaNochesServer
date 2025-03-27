using DataBaseManager;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MilyUnaNochesService.Contracts {
    [ServiceContract]
    public interface ISaleManager {
        [OperationContract]
        SaleResult ProcessSale(Venta sale, List<VentaProducto> details);

        [OperationContract]
        List<Venta> SearchSales(DateTime? date, int? employeeId);

        [OperationContract]
        List<string> ValidateSale(List<VentaProducto> details);
    }

    [DataContract]
    public class Venta {
        [DataMember] public int idVenta { get; set; }
        [DataMember] public int IdEmpleado { get; set; }
        [DataMember] public int? IdCliente { get; set; }
        [DataMember] public string MetodoPago { get; set; }
        [DataMember] public decimal MontoTotal { get; set; }
        [DataMember] public DateTime fecha { get; set; }
        [DataMember] public TimeSpan hora { get; set; }
        [DataMember] public List<VentaProducto> Detalles { get; set; } 

    }

    [DataContract]
    public class VentaProducto {
        [DataMember(Order = 1, IsRequired = true)]
        public int IdProducto { get; set; }

        [DataMember(Order = 2)]
        public string NombreProducto { get; set; }

        [DataMember(Order = 3)]
        public int Cantidad { get; set; }

        [DataMember(Order = 4)]
        public decimal PrecioUnitario { get; set; }

        [DataMember(Order = 5)]
        public decimal Subtotal { get; set; }
    }

    [DataContract]
    public class SaleResult {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public List<string> Errors { get; set; }

        [DataMember]
        public int? SaleId { get; set; }
    }

}