using DataBaseManager.Operations;
using DataBaseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Operations
{
    public class ProductOperationTest
    {
        [Fact]
        public void SaveProductTest()
        {
            var product = new Producto
            {
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
        public void SaveProductTest_WithMissingName()
        {
            var product = new Producto
            {
                codigoProducto = "PROD002",
                nombreProducto = null,
                descripcion = "Producto sin nombre",
                categoria = "Categoría X",
                cantidadStock = 50,
                precioVenta = 20.00m,
                precioCompra = 10.00m,
                imagen = new byte[] { 0x20, 0x20 }
            };

            bool isSaved = ProductOperation.SaveProduct(product);

            Assert.False(isSaved);
        }

        [Fact]
        public void GetProductsTest()
        {
            var product = new Producto
            {
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

        [Fact]
        public void GetProductsTest_MultipleProducts()
        {
            var product1 = new Producto
            {
                codigoProducto = "PROD003",
                nombreProducto = "Test Product 3",
                descripcion = "Tercer producto de prueba",
                categoria = "Categoría Prueba",
                cantidadStock = 150,
                precioVenta = 65.00m,
                precioCompra = 35.00m,
                imagen = new byte[] { 0x40, 0x40 }
            };

            var product2 = new Producto
            {
                codigoProducto = "PROD004",
                nombreProducto = "Test Product 4",
                descripcion = "Cuarto producto de prueba",
                categoria = "Categoría Prueba",
                cantidadStock = 80,
                precioVenta = 85.00m,
                precioCompra = 50.00m,
                imagen = new byte[] { 0x50, 0x50 }
            };

            Assert.True(ProductOperation.SaveProduct(product1));
            Assert.True(ProductOperation.SaveProduct(product2));

            var products = ProductOperation.GetProducts();

            Assert.NotNull(products);
            Assert.True(products.Any(p => p.nombreProducto == "Test Product 3"));
            Assert.True(products.Any(p => p.nombreProducto == "Test Product 4"));
        }

        [Fact]
        public void GetProductByCodeTest_ValidCode()
        {
            var product = new Producto
            {
                codigoProducto = "PROD005",
                nombreProducto = "Producto Código 5",
                descripcion = "Producto de prueba con código específico",
                categoria = "Categoría Test",
                cantidadStock = 120,
                precioVenta = 60.00m,
                precioCompra = 35.00m,
                imagen = new byte[] { 0x60, 0x60 }
            };

            bool isSaved = ProductOperation.SaveProduct(product);
            Assert.True(isSaved);

            var result = ProductOperation.GetProductByCode("PROD005");

            Assert.NotNull(result);
            Assert.Equal("Producto Código 5", result.nombreProducto);
        }

        [Fact]
        public void GetProductByCodeTest_InvalidCodel()
        {
            var result = ProductOperation.GetProductByCode("NO_EXISTE");

            Assert.Null(result);
        }

        [Fact]
        public void CheckStockByCodeTest_SufficientStock()
        {
            var product = new Producto
            {
                codigoProducto = "PROD006",
                nombreProducto = "Producto con Stock",
                descripcion = "Producto de prueba con stock suficiente",
                categoria = "Categoría Test",
                cantidadStock = 50,
                precioVenta = 70.00m,
                precioCompra = 40.00m,
                imagen = new byte[] { 0x66, 0x66 }
            };

            bool isSaved = ProductOperation.SaveProduct(product);
            Assert.True(isSaved);

            bool hasStock = ProductOperation.CheckStockByCode("PROD006", 20);

            Assert.True(hasStock);
        }

        [Fact]
        public void CheckStockByCodeTest_ProductNotFoundOrInsufficientStock()
        {
            bool resultNotFound = ProductOperation.CheckStockByCode("NO_EXISTE", 5);
            Assert.False(resultNotFound);

            var product = new Producto
            {
                codigoProducto = "PROD007",
                nombreProducto = "Producto sin stock suficiente",
                descripcion = "Producto de prueba con poco stock",
                categoria = "Categoría Test",
                cantidadStock = 5,
                precioVenta = 80.00m,
                precioCompra = 45.00m,
                imagen = new byte[] { 0x77, 0x77 }
            };

            bool isSaved = ProductOperation.SaveProduct(product);
            Assert.True(isSaved);

            bool insufficientStock = ProductOperation.CheckStockByCode("PROD007", 10); // pide más de lo que hay

            Assert.False(insufficientStock);
        }

        [Fact]
        public void ValidateProductNameTest_UniqueName()
        {
            string uniqueName = "Nombre Unico Producto";

            bool isValid = ProductOperation.ValidateProductName(uniqueName);

            Assert.True(isValid);
        }

        [Fact]
        public void ValidateProductNameTest_ExistingName()
        {
            var product = new Producto
            {
                codigoProducto = "PROD008",
                nombreProducto = "Producto Existente",
                descripcion = "Producto ya guardado",
                categoria = "Categoría",
                cantidadStock = 10,
                precioVenta = 90.00m,
                precioCompra = 60.00m,
                imagen = new byte[] { 0x88, 0x88 }
            };

            bool isSaved = ProductOperation.SaveProduct(product);
            Assert.True(isSaved);

            bool isValid = ProductOperation.ValidateProductName("Producto Existente");

            Assert.False(isValid);
        }

        [Fact]
        public void UpdateProductTest_ExistingProduct()
        {
            var original = new Producto
            {
                codigoProducto = "PROD009",
                nombreProducto = "Producto Original",
                descripcion = "Descripción original",
                categoria = "Categoría A",
                cantidadStock = 30,
                precioVenta = 100.00m,
                precioCompra = 60.00m,
                imagen = new byte[] { 0x90, 0x90 }
            };

            bool saved = ProductOperation.SaveProduct(original);
            Assert.True(saved);

            var updated = new Producto
            {
                codigoProducto = "PROD009",
                nombreProducto = "Producto Modificado",
                descripcion = "Descripción actualizada",
                categoria = "Categoría B",
                cantidadStock = 50,
                precioVenta = 120.00m,
                precioCompra = 70.00m,
                imagen = new byte[] { 0x91, 0x91 }
            };

            bool updatedResult = ProductOperation.UpdateProduct(updated, "Producto Original");

            Assert.True(updatedResult);

            // Validar que se actualizó
            var productoFinal = ProductOperation.GetProductByCode("PROD009");
            Assert.Equal("Producto Modificado", productoFinal.nombreProducto);
            Assert.Equal(50, productoFinal.cantidadStock);
        }

        [Fact]
        public void UpdateProductTest_NonExistentProduct()
        {
            var producto = new Producto
            {
                codigoProducto = "sfgbersff",
                nombreProducto = "Producto Fantasma",
                descripcion = "No existe en la base",
                categoria = "Categoría X",
                cantidadStock = 10,
                precioVenta = 50.00m,
                precioCompra = 30.00m,
                imagen = new byte[] { 0x99, 0x99 }
            };

            bool result = ProductOperation.UpdateProduct(producto, "Nombre Inexistente");
          
            Assert.False(result);
        }

        [Fact]
        public void DeleteProductTest_ExistingProduct()
        {
            var product = new Producto
            {
                codigoProducto = "PROD011",
                nombreProducto = "Producto Para Eliminar",
                descripcion = "Este producto será eliminado",
                categoria = "Categoría Eliminar",
                cantidadStock = 5,
                precioVenta = 45.00m,
                precioCompra = 25.00m,
                imagen = new byte[] { 0x11, 0x11 }
            };

            bool isSaved = ProductOperation.SaveProduct(product);
            Assert.True(isSaved);

            bool isDeleted = ProductOperation.DeleteProduct("Producto Para Eliminar");

            Assert.True(isDeleted);

            var deletedProduct = ProductOperation.GetProductByCode("PROD011");
            Assert.Null(deletedProduct);
        }

        [Fact]
        public void DeleteProductTest_NonExistentProduct()
        {
            bool isDeleted = ProductOperation.DeleteProduct("Producto Inexistente");

            Assert.False(isDeleted);
        }

        [Fact]
        public void NumProductsTest_WithKnownProducts()
        {
            var p1 = new Producto
            {
                codigoProducto = "PROD012",
                nombreProducto = "Producto A",
                descripcion = "Desc A",
                categoria = "Cat A",
                cantidadStock = 5,
                precioVenta = 20.00m,
                precioCompra = 10.00m,
                imagen = new byte[] { 0x01 }
            };

            var p2 = new Producto
            {
                codigoProducto = "PROD013",
                nombreProducto = "Producto B",
                descripcion = "Desc B",
                categoria = "Cat B",
                cantidadStock = 10,
                precioVenta = 40.00m,
                precioCompra = 30.00m,
                imagen = new byte[] { 0x02 }
            };

            ProductOperation.SaveProduct(p1);
            ProductOperation.SaveProduct(p2);

            int count = ProductOperation.NumProducts();

            Assert.True(count >= 2);
        }
    }
}