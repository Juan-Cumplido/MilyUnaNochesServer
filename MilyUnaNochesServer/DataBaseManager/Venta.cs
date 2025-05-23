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
    
    public partial class Venta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Venta()
        {
            this.VentaProducto = new HashSet<VentaProducto>();
        }
    
        public int idVenta { get; set; }
        public int idEmpleado { get; set; }
        public System.DateTime fecha { get; set; }
        public decimal montoTotal { get; set; }
        public System.TimeSpan hora { get; set; }
        public string metodoPago { get; set; }
        public Nullable<int> idCliente { get; set; }
    
        public virtual Empleado Empleado { get; set; }
        public virtual Cliente Cliente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VentaProducto> VentaProducto { get; set; }
    }
}
