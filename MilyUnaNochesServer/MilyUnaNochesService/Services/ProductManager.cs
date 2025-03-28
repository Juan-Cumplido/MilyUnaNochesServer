using DataBaseManager;
using DataBaseManager.Operations;
using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Services
{
    public partial class MilyUnaNochesService : IProductsManager
    {
        public Task<bool> CheckStockByCodeAsync(string productCode, int quantity) {
            throw new NotImplementedException();
        }

        public Task<Product> GetProductByCodeAsync(string productCode) {
            throw new NotImplementedException();
        }

        public List<Product> GetProducts() {
            try {
                List<Producto> productos = ProductOperation.GetProducts();

                List<Product> products = productos.Select(producto => new Product {
                    IdProducto = producto.idProducto,
                    CodigoProducto = producto.codigoProducto,
                    NombreProducto = producto.nombreProducto,
                    Descripcion = producto.descripcion,
                    Categoria = producto.categoria,
                    Cantidad = producto.cantidadStock,
                    PrecioVenta = producto.precioVenta,
                    PrecioCompra = producto.precioCompra,
                    Imagen = producto.imagen
                }).ToList();

                return products;
            } catch (Exception ex) {
                Console.WriteLine($"Error al obtener los productos: {ex.Message}");
                throw;
            }
        }

        public StockResponse GetProductStock(int productId) {
            throw new NotImplementedException();
        }

        public bool SaveProduct(Product product)
        {
            DataBaseManager.Producto newProduct = new DataBaseManager.Producto()
            {
                codigoProducto = product.CodigoProducto,
                nombreProducto = product.NombreProducto,
                descripcion = product.Descripcion,
                categoria = product.Categoria,
                cantidadStock = product.Cantidad,
                precioVenta = product.PrecioVenta,
                precioCompra = product.PrecioCompra,
                imagen = product.Imagen
            };

            bool insertionResult = ProductOperation.SaveProduct(newProduct);
            return insertionResult;
        }
    }
}
