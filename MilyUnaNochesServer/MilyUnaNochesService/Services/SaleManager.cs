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
                // Validación de stock
                var validationErrors = ValidateSale(details);
                if (validationErrors.Any()) {
                    result.Success = false;
                    result.Errors = validationErrors;
                    return result;
                }

                // Mapeo completo de la venta
                var dbSale = new DataBaseManager.Venta {
                    idEmpleado = sale.IdEmpleado,
                    idCliente = sale.IdCliente,
                    metodoPago = sale.MetodoPago ?? "EFECTIVO",
                    montoTotal = sale.MontoTotal,
                    fecha = DateTime.Now.Date,
                    hora = DateTime.Now.TimeOfDay
                };

                using (var context = new DataBaseManager.MilYUnaNochesEntities()) {
                    // Mapeo completo de los detalles
                    var dbDetails = details.Select(d => {
                        var product = context.Producto.Find(d.IdProducto);
                        return new DataBaseManager.VentaProducto {
                            idProducto = d.IdProducto,
                            cantidadProducto = d.Cantidad,
                            precioVentaHistorico = d.PrecioUnitario, // Usar el precio del cliente
                            precioCompraHistorico = product.precioCompra,
                            // Asegurar que todos los campos requeridos estén mapeados
                        };
                    }).ToList();

                    var operationResult = SaleOperation.RegisterSale(dbSale, dbDetails);

                    if (operationResult == Constants.SuccessOperation) {
                        result.Success = true;
                        result.SaleId = dbSale.idVenta;
                    } else {
                        result.Success = false;
                        result.Errors.Add($"Error en base de datos: {operationResult}");
                    }

                    return result;
                }
            } catch (Exception ex) {
                result.Success = false;
                result.Errors.Add($"Error al procesar la venta: {ex.Message}");
                Console.WriteLine(ex);
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
                        PrecioUnitario = d.precioVentaHistorico, // Usamos precioVentaHistorico
                        PrecioCompra = d.precioCompraHistorico,
                        MargenGanancia = d.precioVentaHistorico - d.precioCompraHistorico,
                        Subtotal = d.cantidadProducto * d.precioVentaHistorico // Calculamos con precio histórico
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