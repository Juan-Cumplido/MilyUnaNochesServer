//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataBaseManager
{
    using System;
    using System.Collections.Generic;
    
    public partial class VentaProducto
    {
        public int idVenta { get; set; }
        public int idProducto { get; set; }
        public int cantidadProducto { get; set; }
        public decimal precioVentaHistorico { get; set; }
        public decimal precioCompraHistorico { get; set; }
        public Nullable<decimal> montoTotal { get; set; }
    
        public virtual Producto Producto { get; set; }
        public virtual Venta Venta { get; set; }
    }
}
