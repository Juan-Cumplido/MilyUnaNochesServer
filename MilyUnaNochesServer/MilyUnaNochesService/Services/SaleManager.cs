using DataBaseManager.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using MilyUnaNochesService.Contracts;

namespace MilyUnaNochesService.Services {
    public partial class MilyUnaNochesService : ISaleManager {
        public bool ProcessSale(Venta sale, List<VentaProducto> details) {
            try {
                // Validación previa
                if (details == null || details.Count == 0)
                    throw new ArgumentException("La venta debe contener al menos un producto");

                // Convertir `Contracts.Venta` a `DataBaseManager.Venta`
                var dbSale = new DataBaseManager.Venta {
                    idEmpleado = sale.IdEmpleado,
                    idCliente = sale.IdCliente,
                    metodoPago = sale.MetodoPago,
                    montoTotal = sale.MontoTotal
                };

                // Convertir `Contracts.DetalleVenta` a `DataBaseManager.VentaProducto`
                var dbDetails = details.Select(d => new DataBaseManager.VentaProducto {
                    idProducto = d.IdProducto,
                    cantidadProducto = d.Cantidad,
                    montoProducto = d.PrecioUnitario
                }).ToList();

                return SaleOperation.RegisterSale(dbSale, dbDetails);
            } catch (Exception ex) {
                throw new FaultException("Error al procesar la venta");
            }
        }

        public List<Venta> SearchSales(DateTime? date, int? employeeId) {
            try {
                var sales = SaleOperation.GetSales(date, employeeId);

                // Convertir `DataBaseManager.Venta` a `Contracts.Venta`
                return sales.Select(s => new Venta {
                    IdEmpleado = s.idEmpleado,
                    IdCliente = s.idCliente,
                    MetodoPago = s.metodoPago,
                    MontoTotal = s.montoTotal
                }).ToList();
            } catch (Exception ex) {
                throw new FaultException("Error al buscar ventas");
            }
        }

        public bool ValidateSale(List<VentaProducto> details) {
            try {
                if (details == null || details.Count == 0)
                    return false;

                // Convertir `Contracts.DetalleVenta` a `DataBaseManager.VentaProducto`
                var dbDetails = details.Select(d => new DataBaseManager.VentaProducto {
                    idProducto = d.IdProducto,
                    cantidadProducto = d.Cantidad,
                    montoProducto = d.PrecioUnitario
                }).ToList();

                return SaleOperation.ValidateStock(dbDetails);
            } catch (Exception ex) {
                throw new FaultException("Error al validar stock");
            }
        }

    }
}
