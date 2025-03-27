using MilyUnaNochesService.Utilities;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace DataBaseManager.Operations {
    public static class SaleOperation {
        /* Valida que todos los productos en el detalle tengan stock suficiente */
        public static int ValidateStock(List<VentaProducto> details) {
            LoggerManager logger = new LoggerManager(typeof(SaleOperation));

            try {
                using (var db = new MilYUnaNochesEntities()) {
                    bool hasInsufficientStock = details.Any(detail => {
                        var stock = db.Producto
                                    .Where(p => p.idProducto == detail.idProducto)
                                    .Select(p => (int?)p.cantidadStock)
                                    .FirstOrDefault() ?? 0;
                        return stock < detail.cantidadProducto;
                    });

                    return hasInsufficientStock ? Constants.NoDataMatches : Constants.DataMatches;
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                return Constants.ErrorOperation;
            } catch (EntityException entityException) {
                logger.LogFatal(entityException);
                return Constants.ErrorOperation;
            }
        }

        /* Registra una venta y actualiza el stock (transaccional) */
        public static int RegisterSale(Venta sale, List<VentaProducto> details) {
            LoggerManager logger = new LoggerManager(typeof(SaleOperation));

            try {
                using (var db = new MilYUnaNochesEntities())
                using (var transaction = db.Database.BeginTransaction()) {
                    try {
                        decimal totalAmount = 0;
                        List<VentaProducto> saleDetails = new List<VentaProducto>();

                        foreach (var detail in details) {
                            var product = db.Producto.SingleOrDefault(p => p.idProducto == detail.idProducto);
                            if (product == null) {
                                return Constants.NoDataMatches;
                            }

                            if (product.cantidadStock < detail.cantidadProducto) {
                                return Constants.NoDataMatches;
                            }

                            decimal unitPrice = product.precioVenta;
                            totalAmount += unitPrice * detail.cantidadProducto;

                            saleDetails.Add(new VentaProducto {
                                idProducto = detail.idProducto,
                                cantidadProducto = detail.cantidadProducto,
                                montoProducto = unitPrice
                            });

                            product.cantidadStock -= detail.cantidadProducto;
                        }

                        var newSale = new Venta {
                            idEmpleado = sale.idEmpleado,
                            idCliente = sale.idCliente,
                            fecha = DateTime.Now.Date,
                            hora = DateTime.Now.TimeOfDay,
                            metodoPago = sale.metodoPago,
                            montoTotal = totalAmount
                        };
                        db.Venta.Add(newSale);
                        db.SaveChanges();

                        // Asignar el ID de venta a los detalles y agregarlos a la base de datos
                        saleDetails.ForEach(detail => detail.idVenta = newSale.idVenta);
                        db.VentaProducto.AddRange(saleDetails);
                        db.SaveChanges();

                        transaction.Commit();
                        return Constants.SuccessOperation;
                    } catch (DbUpdateException updateException) {
                        logger.LogWarn(updateException);
                        transaction.Rollback();
                        return Constants.ErrorOperation;
                    } catch (SqlException sqlException) {
                        logger.LogError(sqlException);
                        transaction.Rollback();
                        return Constants.ErrorOperation;
                    }
                }
            } catch (EntityException entityException) {
                logger.LogFatal(entityException);
                return Constants.ErrorOperation;
            }
        }

        /* Busca ventas por fecha y/o empleado */
        public static SalesResult GetSales(DateTime? date, int? employeeId) {
            LoggerManager logger = new LoggerManager(typeof(SaleOperation));
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

                    var sales = query.OrderByDescending(v => v.fecha)
                                   .ThenByDescending(v => v.hora)
                                   .ToList();

                    if (sales.Any()) {
                        result.Sales = sales;
                        result.ResultCode = Constants.DataMatches;
                    } else {
                        result.ResultCode = Constants.NoDataMatches;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                result.ResultCode = Constants.ErrorOperation;
            } catch (EntityException entityException) {
                logger.LogFatal(entityException);
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