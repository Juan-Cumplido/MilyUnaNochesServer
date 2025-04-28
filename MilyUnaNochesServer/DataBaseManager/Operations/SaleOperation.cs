using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace DataBaseManager.Operations {
    public static class SaleOperation {
        public static int ValidateStock(List<VentaProducto> details) {
            using (var context = new MilYUnaNochesEntities()) {
                foreach (var item in details) {
                    var product = context.Producto.Find(item.idProducto);
                    if (product == null || product.cantidadStock < item.cantidadProducto) {
                        return Constants.NoDataMatches;
                    }
                }
                return Constants.DataMatches;
            }
        }

        public static int RegisterSale(Venta sale, List<VentaProducto> details) {
            var logger = new LoggerManager(typeof(SaleOperation));

            try {
                using (var db = new MilYUnaNochesEntities())
                using (var transaction = db.Database.BeginTransaction()) {
                    try {
                        decimal totalAmount = 0;

                        // Primero validar todos los productos y calcular el total
                        foreach (var detail in details) {
                            var product = db.Producto.SingleOrDefault(p => p.idProducto == detail.idProducto);
                            if (product == null || product.cantidadStock < detail.cantidadProducto) {
                                return Constants.NoDataMatches;
                            }
                            totalAmount += product.precioVenta * detail.cantidadProducto;
                        }

                        // Crear y guardar la venta primero
                        var newSale = new Venta {
                            idEmpleado = sale.idEmpleado,
                            idCliente = sale.idCliente,
                            fecha = DateTime.Now.Date,
                            hora = DateTime.Now.TimeOfDay,
                            metodoPago = sale.metodoPago,
                            montoTotal = totalAmount
                        };

                        db.Venta.Add(newSale);
                        db.SaveChanges(); // Esto asigna el idVenta

                        // Ahora crear los detalles con el idVenta asignado
                        var saleDetails = new List<VentaProducto>();
                        foreach (var detail in details) {
                            var product = db.Producto.Single(p => p.idProducto == detail.idProducto);

                            saleDetails.Add(new VentaProducto {
                                idVenta = newSale.idVenta, // Asignar explícitamente el idVenta
                                idProducto = detail.idProducto,
                                cantidadProducto = detail.cantidadProducto,
                                precioVentaHistorico = product.precioVenta,
                                precioCompraHistorico = product.precioCompra
                            });

                            // Actualizar stock
                            product.cantidadStock -= detail.cantidadProducto;
                        }

                        // Agregar todos los detalles de una vez
                        db.VentaProducto.AddRange(saleDetails);
                        db.SaveChanges();

                        transaction.Commit();
                        return Constants.SuccessOperation;
                    } catch (DbUpdateException ex) {
                        logger.LogWarn(ex);
                        transaction.Rollback();
                        return Constants.ErrorOperation;
                    } catch (SqlException ex) {
                        logger.LogError(ex);
                        transaction.Rollback();
                        return Constants.ErrorOperation;
                    }
                }
            } catch (EntityException ex) {
                logger.LogFatal(ex);
                return Constants.ErrorOperation;
            }
        }

        public static SalesResult GetSales(DateTime? date, int? employeeId) {
            var logger = new LoggerManager(typeof(SaleOperation));
            var result = new SalesResult();

            try {
                using (var db = new MilYUnaNochesEntities()) {
                    var query = db.Venta
                        .Include(v => v.VentaProducto)
                        .Include(v => v.VentaProducto.Select(vp => vp.Producto))
                        .AsNoTracking();

                    if (date.HasValue)
                        query = query.Where(v => DbFunctions.TruncateTime(v.fecha) == date.Value.Date);

                    if (employeeId.HasValue)
                        query = query.Where(v => v.idEmpleado == employeeId.Value);

                    result.Sales = query.OrderByDescending(v => v.fecha)
                                      .ThenByDescending(v => v.hora)
                                      .ToList();

                    result.ResultCode = result.Sales.Any() ? Constants.DataMatches : Constants.NoDataMatches;
                }
            } catch (SqlException ex) {
                logger.LogError(ex);
                result.ResultCode = Constants.ErrorOperation;
            } catch (EntityException ex) {
                logger.LogFatal(ex);
                result.ResultCode = Constants.ErrorOperation;
            }

            return result;
        }
    }

    public class SalesResult {
        public List<Venta> Sales { get; set; }
        public int ResultCode { get; set; }
    }
}