using DataBaseManager.Operations;
using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;

namespace MilyUnaNochesService.Services {
    public partial class MilyUnaNochesService : ISaleManager {
        public SaleResult ProcessSale(Venta sale, List<VentaProducto> details) {
            var result = new SaleResult();

            try {
                // Primero validar el stock
                var validationErrors = ValidateSale(details);
                if (validationErrors.Any()) {
                    result.Success = false;
                    result.Errors = validationErrors;
                    return result;
                }

                var dbSale = new DataBaseManager.Venta {
                    idEmpleado = sale.IdEmpleado,
                    idCliente = sale.IdCliente,
                    metodoPago = sale.MetodoPago ?? "EFECTIVO",
                    montoTotal = sale.MontoTotal
                };

                var dbDetails = details.Select(d => new DataBaseManager.VentaProducto {
                    idProducto = d.IdProducto,
                    cantidadProducto = d.Cantidad,
                    montoProducto = d.PrecioUnitario
                }).ToList();

                var operationResult = SaleOperation.RegisterSale(dbSale, dbDetails);

                if (operationResult != Constants.SuccessOperation)
                    throw new ApplicationException($"Error en RegisterSale. Código: {operationResult}");

                result.Success = true;
                return result;
            } catch (Exception ex) {
                result.Success = false;
                result.Errors = new List<string> { $"Error al procesar la venta: {ex.Message}" };
                return result;
            }
        }

        public List<Venta> SearchSales(DateTime? date, int? employeeId) {
            try {
                var salesResult = SaleOperation.GetSales(date, employeeId);

                if (salesResult.ResultCode != Constants.DataMatches)
                    return new List<Venta>();

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

        public List<string> ValidateSale(List<VentaProducto> details) {
            {
                var errors = new List<string>();

                if (details == null || !details.Any()) {
                    errors.Add("La venta no contiene productos");
                    return errors;
                }

                using (var context = new DataBaseManager.MilYUnaNochesEntities()) {
                    foreach (var d in details) {
                        if (d.IdProducto <= 0) {
                            errors.Add($"ID de producto inválido: {d.IdProducto}");
                            continue;
                        }

                        var product = context.Producto.FirstOrDefault(p => p.idProducto == d.IdProducto);

                        if (product == null) {
                            errors.Add($"Producto con ID {d.IdProducto} no encontrado");
                            continue;
                        }

                        if (product.cantidadStock < d.Cantidad) {
                            errors.Add($"{product.nombreProducto}: Stock insuficiente (Disponible: {product.cantidadStock}, Requerido: {d.Cantidad})");
                        }
                    }
                }

                return errors;
            }
        }
    }
}