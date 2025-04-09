using DataBaseManager;
using DataBaseManager.Operations;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests.Operations {
    public class SaleOperationTest : IDisposable {
        // Datos conocidos del script SQL
        private const string TestEmployeeEmail = "carlos.gomez@empresa.com";
        private const string TestCustomerEmail = "maria.hernandez@email.com";
        private const string TestProductCode = "MAT-001";

        private int _testEmployeeId;
        private int _testCustomerId;
        private int _testProductId;

        public SaleOperationTest() {
            // Obtener IDs de los datos insertados por el script
            using (var db = new MilYUnaNochesEntities()) {
                _testEmployeeId = db.Empleado
                    .Where(e => e.Usuario.correo == TestEmployeeEmail)
                    .Select(e => e.idEmpleado)
                    .FirstOrDefault();

                _testCustomerId = db.Cliente
                    .Where(c => c.Usuario.correo == TestCustomerEmail)
                    .Select(c => c.idCliente)
                    .FirstOrDefault();

                _testProductId = db.Producto
                    .Where(p => p.codigoProducto == TestProductCode)
                    .Select(p => p.idProducto)
                    .FirstOrDefault();
            }
        }

        public void Dispose() {
            // No necesitamos limpiar porque los datos son persistentes
            // y pueden ser reutilizados para otras pruebas
        }

        [Fact]
        public void RegisterSale_WithValidData_ReturnsSuccess() {
            // Arrange
            var sale = new Venta {
                idEmpleado = _testEmployeeId,
                idCliente = _testCustomerId,
                metodoPago = "EFECTIVO"
            };

            var details = new List<VentaProducto>
            {
                new VentaProducto
                {
                    idProducto = _testProductId,
                    cantidadProducto = 2 // Cantidad menor al stock disponible
                }
            };

            // Act
            var result = SaleOperation.RegisterSale(sale, details);

            // Assert
            Assert.Equal(Constants.SuccessOperation, result);

            // Verificar que el stock se actualizó
            using (var db = new MilYUnaNochesEntities()) {
                var product = db.Producto.Find(_testProductId);
                var newStock = 50 - 3 - 2; // 3 del script inicial, 2 de esta prueba
                Assert.Equal(newStock, product.cantidadStock);
            }
        }

        [Fact]
        public void ValidateStock_WithSufficientStock_ReturnsDataMatches() {
            // Arrange
            var details = new List<VentaProducto>
            {
                new VentaProducto
                {
                    idProducto = _testProductId,
                    cantidadProducto = 10 // Menos que el stock disponible
                }
            };

            // Act
            var result = SaleOperation.ValidateStock(details);

            // Assert
            Assert.Equal(Constants.DataMatches, result);
        }

        [Fact]
        public void ValidateStock_WithInsufficientStock_ReturnsNoDataMatches() {
            // Arrange
            var details = new List<VentaProducto>
            {
                new VentaProducto
                {
                    idProducto = _testProductId,
                    cantidadProducto = 100 // Más que el stock disponible
                }
            };

            // Act
            var result = SaleOperation.ValidateStock(details);

            // Assert
            Assert.Equal(Constants.NoDataMatches, result);
        }

        [Fact]
        public void GetSales_WithEmployeeFilter_ReturnsFilteredSales() {
            // Act
            var result = SaleOperation.GetSales(null, _testEmployeeId);

            // Assert
            Assert.Equal(Constants.DataMatches, result.ResultCode);
            Assert.True(result.Sales.Count > 0);
            Assert.All(result.Sales, s => Assert.Equal(_testEmployeeId, s.idEmpleado));
        }
    }
}