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
        public static bool ValidateStock(List<VentaProducto> details) {
            using (var db = new MilYUnaNochesEntities()) {
                return !details.Any(detail => {
                    var stock = db.Producto
                                  .Where(p => p.idProducto == detail.idProducto)
                                  .Select(p => (int?)p.cantidadStock)
                                  .FirstOrDefault() ?? 0;
                    return stock < detail.cantidadProducto;
                });
            }
        }

        /* Registra una venta y actualiza el stock (transaccional) */
        public static bool RegisterSale(Venta sale, List<VentaProducto> details) {
            using (var db = new MilYUnaNochesEntities())
            using (var transaction = db.Database.BeginTransaction()) {
                try {
                    decimal totalAmount = 0;
                    List<VentaProducto> saleDetails = new List<VentaProducto>();

                    foreach (var detail in details) {
                        var product = db.Producto.SingleOrDefault(p => p.idProducto == detail.idProducto);
                        if (product == null) {
                            throw new Exception($"El producto con ID {detail.idProducto} no existe.");
                        }

                        if (product.cantidadStock < detail.cantidadProducto) {
                            throw new Exception($"Stock insuficiente para el producto {detail.idProducto}.");
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
                    return true;
                } catch (SqlException ex) {
                    transaction.Rollback();
                    LoggerManager logger = new LoggerManager(typeof(SaleOperation));
                    logger.LogError($"SQL Error registering sale: {ex.Message}", ex);
                    throw;
                } catch (DbUpdateException ex) {
                    transaction.Rollback();
                    LoggerManager logger = new LoggerManager(typeof(SaleOperation));
                    logger.LogError($"Database update error: {ex.Message}", ex);
                    throw;
                } catch (EntityException ex) {
                    transaction.Rollback();
                    LoggerManager logger = new LoggerManager(typeof(SaleOperation));
                    logger.LogError($"Entity Framework error: {ex.Message}", ex);
                    throw;
                } catch (Exception ex) {
                    transaction.Rollback();
                    LoggerManager logger = new LoggerManager(typeof(SaleOperation));
                    logger.LogError($"Unexpected error in RegisterSale: {ex.Message}", ex);
                    throw;
                }
            }
        }

        /* Busca ventas por fecha y/o empleado */
        public static List<Venta> GetSales(DateTime? date, int? employeeId) {
            using (var db = new MilYUnaNochesEntities()) {
                var query = db.Venta
                              .Include(v => v.VentaProducto)
                              .AsNoTracking();

                if (date.HasValue)
                    query = query.Where(v => DbFunctions.TruncateTime(v.fecha) == date.Value.Date);
                if (employeeId.HasValue)
                    query = query.Where(v => v.idEmpleado == employeeId.Value);

                return query.ToList();
            }
        }
    }
}
