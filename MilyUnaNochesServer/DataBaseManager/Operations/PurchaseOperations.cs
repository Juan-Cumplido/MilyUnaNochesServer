using DataBaseManager.Logic;
using log4net.Repository.Hierarchy;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Xml;

namespace DataBaseManager.Operations {
    public static class PurchaseOperations {
        /// <summary>
        /// Guarda una compra (encabezado y detalles) en la base de datos.
        /// </summary>
        /// <param name="purchase">Objeto con la información de la compra</param>
        /// <returns>Constante indicando éxito o error de la operación</returns>
        public static int SavePurchase(RegisterPurchaseDTO purchase) {
            int result = Constants.ErrorOperation;
            LoggerManager logger = new LoggerManager(typeof(PurchaseOperations));
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    using (var transaction = db.Database.BeginTransaction()) {
                        try {
                            CompraProveedor newPurchase = new CompraProveedor() {
                                idProveedor = purchase.IdProveedor,
                                contactoProveedor = purchase.ContactoProveedor,
                                fecha = purchase.Fecha,
                                hora = purchase.Hora,
                                metodoPago = purchase.PayMethod,
                                montoTotal = purchase.MontoTotal
                            };
                            db.CompraProveedor.Add(newPurchase);
                            db.SaveChanges();

                            int purchaseId = newPurchase.idCompra;

                            foreach (var prod in purchase.Products) {
                                var producto = db.Producto.Find(prod.IdProducto);
                                if (producto == null) {
                                    transaction.Rollback();
                                    throw new KeyNotFoundException($"Producto ID {prod.IdProducto} no encontrado");
                                }

                                CompraProducto purchaseDetail = new CompraProducto() {
                                    idCompra = purchaseId,
                                    idProducto = prod.IdProducto,
                                    cantidad = prod.Cantidad,
                                    montoProducto = prod.MontoProducto
                                };
                                db.CompraProducto.Add(purchaseDetail);

                                producto.cantidadStock += prod.Cantidad;
                            }
                            db.SaveChanges();
                            transaction.Commit();
                            result = Constants.SuccessOperation;
                        } catch (DbEntityValidationException dbEx) {
                            transaction.Rollback();
                            result = Constants.ErrorOperation;
                            StringBuilder sb = new StringBuilder();
                            foreach (var entityValidationErrors in dbEx.EntityValidationErrors) {
                                foreach (var validationError in entityValidationErrors.ValidationErrors) {
                                    sb.AppendLine($"Entidad: {entityValidationErrors.Entry.Entity.GetType().Name}, Propiedad: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                                }
                            }
                            logger.LogError($"DbEntityValidationException: Errores de validación: {sb.ToString()}", dbEx);
                        } catch (EntityException entityException) {
                            transaction.Rollback();
                            result = Constants.ErrorOperation;
                            string innerMessage = entityException.InnerException != null
                                ? entityException.InnerException.Message
                                : "No hay inner exception";
                            logger.LogError($"EntityException: Error registrando la compra: {entityException.Message}. InnerException: {innerMessage}", entityException);
                        } catch (Exception ex) {
                            transaction.Rollback();
                            result = Constants.ErrorOperation;
                            logger.LogError($"Excepcion: Error registrando la compra: {ex.Message}", ex);
                        }
                    }
                }
            } catch (Exception ex) {
                result = Constants.ErrorOperation;
                logger.LogError($"EntityException: Unable to access the database: {ex.Message}", ex);
            }
            return result;
        }

            public static List<ConsultPurchaseDTO> GetRegisteredPurchases() {
                LoggerManager logger = new LoggerManager(typeof(PurchaseOperations));
                List<ConsultPurchaseDTO> registeredPurchases = new List<ConsultPurchaseDTO>();

                try {
                    using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                        var result = (from compra in db.CompraProveedor
                                      join proveedor in db.Proveedor
                                      on compra.idProveedor equals proveedor.idProveedor
                                      select new {
                                          compra,
                                          proveedor,
                                          productos = (from cp in db.CompraProducto
                                                       join prod in db.Producto on cp.idProducto equals prod.idProducto
                                                       where cp.idCompra == compra.idCompra
                                                       select new {
                                                           Nombre = prod.nombreProducto,
                                                           Cantidad = cp.cantidad
                                                       }).ToList()
                                      }).ToList();

                        foreach (var item in result) {
                            string productosTexto = string.Join(", ", item.productos.Select(p => $"{p.Nombre} (x{p.Cantidad})"));

                            registeredPurchases.Add(new ConsultPurchaseDTO {
                                providerName = item.proveedor.nombreProveedor,
                                providerContact = item.compra.contactoProveedor,
                                Fecha = item.compra.fecha,
                                Hora = item.compra.hora,
                                purchasedProducts = productosTexto,
                                payMethod = item.compra.metodoPago,
                                amountPaid = item.compra.montoTotal
                            });
                        }
                    }
                } catch (SqlException sqlException) {
                    logger.LogError($"SQLException: No se pudo acceder a la base de datos: {sqlException.Message}", sqlException);
                } catch (Exception exception) {
                    logger.LogError($"Exception: Error general: {exception.Message}", exception);
                }

                return registeredPurchases;
            }
        }
    }
