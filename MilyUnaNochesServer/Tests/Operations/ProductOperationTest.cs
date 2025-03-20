using DataBaseManager.Operations;
using DataBaseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Operations {
    public class ProductOperationTest {
        [Fact]
        public void SaveProductTest() {
            var product = new Producto {
                codigoProducto = "PROD001",
                nombreProducto = "Test Product",
                descripcion = "Este es un producto de prueba",
                categoria = "Test Category",
                cantidadStock = 100,
                precioVenta = 50.00m,
                precioCompra = 30.00m,
                imagen = new byte[] { 0x20, 0x20 }  
            };


            bool isSaved = ProductOperation.SaveProduct(product);

            Assert.True(isSaved);
        }

        [Fact]
        public void GetProductsTest() {
            var product = new Producto {
                codigoProducto = "PROD002",
                nombreProducto = "Test Product 2",
                descripcion = "Otro producto de prueba",
                categoria = "Test Category",
                cantidadStock = 200,
                precioVenta = 75.00m,
                precioCompra = 40.00m,
                imagen = new byte[] { 0x30, 0x30 }
            };

            bool isSaved = ProductOperation.SaveProduct(product);
            Assert.True(isSaved);

            var products = ProductOperation.GetProducts();

            Assert.NotNull(products);
            Assert.True(products.Any(p => p.nombreProducto == "Test Product 2"));
        }
    }
}