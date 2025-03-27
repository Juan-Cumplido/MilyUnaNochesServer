﻿using DataBaseManager;
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
        public List<Product> GetProducts()
        {
            try
            {
                List<Producto> productos = ProductOperation.GetProducts();

                List<Product> products = productos.Select(producto => new Product
                {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los productos: {ex.Message}");
                throw; 
            }
        }
        public async Task<Product> GetProductByCodeAsync(string productCode) {
            return await Task.Run(() =>
            {
                var producto = ProductOperation.GetProductByCode(productCode);
                if (producto == null) return null;

                return new Product {
                    IdProducto = producto.idProducto,
                    CodigoProducto = producto.codigoProducto,
                    NombreProducto = producto.nombreProducto,
                    Descripcion = producto.descripcion,
                    Categoria = producto.categoria,
                    Cantidad = producto.cantidadStock,
                    PrecioVenta = producto.precioVenta,
                    PrecioCompra = producto.precioCompra,
                    Imagen = producto.imagen
                };
            });
        }

        public async Task<bool> CheckStockByCodeAsync(string productCode, int quantity) {
            return await Task.Run(() =>
            {
                var result = ProductOperation.CheckStockByCode(productCode, quantity);
                Console.WriteLine($"Resultado CheckStock - Código: {productCode}, Cantidad: {quantity}, Resultado: {result}");
                return result;
            });
        }
        public StockResponse GetProductStock(int productId) {
            try {
                using (var db = new MilYUnaNochesEntities()) {
                    var product = db.Producto.Find(productId);
                    if (product == null)
                        return new StockResponse { Success = false, Message = "Producto no encontrado" };

                    return new StockResponse {
                        Success = true,
                        Stock = product.cantidadStock,
                        Message = $"Stock actual: {product.cantidadStock}"
                    };
                }
            } catch (Exception ex) {
                return new StockResponse {
                    Success = false,
                    Message = $"Error al consultar stock: {ex.Message}"
                };

        public bool ValidateProductName(string productName)
        {
            try
            {
                bool exist = ProductOperation.ValidateProductName(productName);
                return exist;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar el nombre del producto: {ex.Message}");
                throw;
            }
        }
    }
}
