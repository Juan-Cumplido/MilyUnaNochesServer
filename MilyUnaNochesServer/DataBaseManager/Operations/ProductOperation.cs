using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

namespace DataBaseManager.Operations {
    public static class ProductOperation {

        public static bool SaveProduct(Producto producto) {
            LoggerManager logger = new LoggerManager(typeof(ProductOperation));
            bool isInserted = false;

            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    db.Producto.Add(producto);
                    db.SaveChanges();
                    isInserted = true;
                }
            } catch (EntityException entityException) {
                logger.LogError($"DbEntityValidationException: Error trying to register product. Exception: {entityException.Message}", entityException);
            } catch (Exception exception) {
                logger.LogError($"Exception: Error trying to register product. Exception: {exception.Message}", exception);
            }

            return isInserted;
        }

        public static List<Producto> GetProducts() 
        {
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    var productosDb = db.Producto.ToList();

                    List<Producto> productos = productosDb.Select(productoDb => new Producto {
                        codigoProducto = productoDb.codigoProducto,
                        nombreProducto = productoDb.nombreProducto,
                        descripcion = productoDb.descripcion,
                        categoria = productoDb.categoria,
                        cantidadStock = productoDb.cantidadStock,
                        precioVenta = productoDb.precioVenta,
                        precioCompra = productoDb.precioCompra,
                        imagen = productoDb.imagen
                    }).ToList();

                    return productos;
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error al obtener los productos: {ex.Message}");
                throw;
            }
        }

        public static Producto GetProductByCode(string code) {
            var logger = new LoggerManager(typeof(ProductOperation));

            try {
                using (var db = new MilYUnaNochesEntities()) {
                    return db.Producto.FirstOrDefault(p => p.codigoProducto == code);
                }
            } catch (EntityException ex) {
                logger.LogError($"Error al buscar producto: {code}", ex);
                return null;
            } catch (Exception ex) {
                logger.LogError($"Error inesperado: {code}", ex);
                return null;
            }
        }

        public static bool CheckStockByCode(string productCode, int requiredQuantity) {
            var logger = new LoggerManager(typeof(ProductOperation));

            try {
                using (var db = new MilYUnaNochesEntities()) {
                    var producto = db.Producto.FirstOrDefault(p => p.codigoProducto == productCode);

                    if (producto == null) {
                        Console.WriteLine($"Producto no encontrado: {productCode}");
                        return false;
                    }

                    logger.LogInfo($"Verificación stock - Código: {productCode}, Stock: {producto.cantidadStock}, Requerido: {requiredQuantity}");
                    return producto.cantidadStock >= requiredQuantity;
                }
            } catch (Exception ex) {
                logger.LogError($"Error al verificar stock: {productCode}", ex);
                return false;
            }
        }
        public static bool ValidateProductName(string productName)
        {
            try
            {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities())
                {
                    bool exist = db.Producto
                                  .Any(p => p.nombreProducto.ToLower() == productName.ToLower());

                    return !exist;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar el nombre del producto: {ex.Message}");
                throw;
            }
        }
    }
}
