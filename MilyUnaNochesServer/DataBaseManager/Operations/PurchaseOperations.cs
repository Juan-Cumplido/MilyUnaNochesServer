using DataBaseManager.Logic;
using log4net.Repository.Hierarchy;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager.Operations {
    public static class PurchaseOperations {
        /// <summary>
        /// Guarda una compra (encabezado y detalles) en la base de datos.
        /// </summary>
        /// <param name="purchase">Objeto con la información de la compra</param>
        /// <returns>Constante indicando éxito o error de la operación</returns>

        public static int SavePurchase(RegisterPurchaseDTO purchase) {
            int result = Constants.ErrorOperation;
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    using (var transaction = db.Database.BeginTransaction()) {
                        try {
                            CompraProveedor newPurchase = new CompraProveedor() {
                                idProveedor = purchase.IdProveedor,
                                fecha = purchase.Fecha,
                                hora = purchase.Hora,
                                metodoPago = purchase.PayMethod,
                                montoTotal = purchase.MontoTotal
                            };
                            db.CompraProveedor.Add(newPurchase);
                            db.SaveChanges();

                            int purchaseId = newPurchase.idCompra;

                            foreach (var prod in purchase.Products) {
                                CompraProducto purchaseDetail = new CompraProducto() {
                                    idCompra = purchaseId,
                                    idProducto = prod.IdProducto,
                                    cantidad = prod.Cantidad,
                                    montoProducto = prod.MontoProducto
                                };
                                db.CompraProducto.Add(purchaseDetail);
                            }
                            db.SaveChanges();
                            transaction.Commit();

                            result = Constants.SuccessOperation;

                        } catch (Exception ex) {
                            transaction.Rollback();
                            result = Constants.ErrorOperation;
                            //logger.LogError($"EntityException: An error has ocurred trying to register the Purchase: {entityException.Message}", entityException);
                        }
                    }
                }
            } catch (Exception ex) {
                result = Constants.ErrorOperation;
                //logger.LogError($"EntityException: Unable to access the database: {entityException.Message}", entityException);
            }
            return result;
        }
    }
}
