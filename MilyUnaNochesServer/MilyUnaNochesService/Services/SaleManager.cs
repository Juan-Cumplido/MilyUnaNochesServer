using DataBaseManager.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Utilities;

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

                // Llamar a la operación para registrar la venta
                int result = SaleOperation.RegisterSale(dbSale, dbDetails);

                // Si la operación es exitosa
                return result == Constants.SuccessOperation;
            } catch (Exception ex) {
                throw new FaultException("Error al procesar la venta");
            }
        }

        public List<Venta> SearchSales(DateTime? date, int? employeeId) {
            try {
                // Obtener ventas desde la base de datos
                var salesResult = SaleOperation.GetSales(date, employeeId);

                if (salesResult.ResultCode != Constants.DataMatches)
                    return new List<Venta>(); // No se encontraron ventas

                // Convertir las ventas y detalles
                return salesResult.Sales.Select(s => new Venta {
                    idVenta = s.idVenta,
                    IdEmpleado = s.idEmpleado,
                    IdCliente = s.idCliente,
                    MetodoPago = s.metodoPago,
                    MontoTotal = s.montoTotal,
                    fecha = s.fecha,
                    hora = s.hora,
                    Detalles = s.VentaProducto.Select(d => new VentaProducto {
                        IdProducto = d.idProducto,
                        NombreProducto = d.Producto?.nombreProducto ?? "Desconocido",
                        Cantidad = d.cantidadProducto,
                        PrecioUnitario = d.montoProducto / d.cantidadProducto,
                        Subtotal = d.cantidadProducto * d.montoProducto
                    }).ToList()
                }).ToList();
            } catch (Exception ex) {
                throw new FaultException("Error al buscar ventas: " + ex.Message);
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

                // Validar stock en la base de datos
                int result = SaleOperation.ValidateStock(dbDetails);

                // Si la operación es exitosa
                return result == Constants.DataMatches;
            } catch (Exception ex) {
                throw new FaultException("Error al validar stock");
            }
        }
    }
}
