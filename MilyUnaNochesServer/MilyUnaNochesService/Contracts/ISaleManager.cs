using DataBaseManager;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MilyUnaNochesService.Contracts {
    [ServiceContract]
    public interface ISaleManager {
        [OperationContract]
        bool ProcessSale(Venta sale, List<VentaProducto> details);

        [OperationContract]
        List<Venta> SearchSales(DateTime? date, int? employeeId);

        [OperationContract]
        bool ValidateSale(List<VentaProducto> details);
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
        [DataMember] public List<VentaProducto> Detalles { get; set; } // Nueva propiedad para los detalles

    }

    [DataContract]
    public class VentaProducto {
        [DataMember] public int IdProducto { get; set; }
        [DataMember] public string NombreProducto { get; set; } // Nuevo campo
        [DataMember] public int Cantidad { get; set; }
        [DataMember] public decimal PrecioUnitario { get; set; }
        [DataMember] public decimal Subtotal { get; set; } // Nuevo campo
    }



}