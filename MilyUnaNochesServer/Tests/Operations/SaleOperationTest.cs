using DataBaseManager;
using DataBaseManager.Operations;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests.Operations {
    public class SaleOperationTest : IDisposable {
        private int _employeeId;
        private int _clientId;
        private int _addressId;
        private List<int> _productIds = new List<int>();
        private List<string> _productNames = new List<string>();
        private readonly UserOperation userOperation;

        public SaleOperationTest() {
            userOperation = new UserOperation();
            CreateTestEnvironment();
        }

        public void Dispose() {
            CleanupTestEnvironment();
        }

        private void CreateTestEnvironment() {
            Console.WriteLine("Iniciando entorno de prueba...");

            var user = new Usuario {
                nombre = "TestEmployee1",
                primerApellido = "EmpleadoApellido1",
                segundoApellido = "EmpleadoApellido2",
                correo = "testemployee@example.com",
                telefono = "0987654321",
                estadoUsuario = "Active"
            };

            var address = new Direccion {
                calle = "Test Calle Emp1",
                numero = "10",
                codigoPostal = "CP10",
                ciudad = "Test Ciudad"
            };

            var employee = new Empleado {
                tipoEmpleado = "Staff"
            };

            var access = new Acceso {
                usuario = "EmpUser1",
                contraseña = "EmpPass"
            };

            int addResult = userOperation.addEmployee(user, address, employee, access);
            Console.WriteLine($"Resultado creación empleado: {addResult}");

            using (var db = new MilYUnaNochesEntities()) {
                var emp = db.Empleado.FirstOrDefault(e => e.Usuario.correo == "testemployee@example.com");
                if (emp != null) {
                    _employeeId = emp.idEmpleado;
                    Console.WriteLine($"Empleado insertado con ID: {_employeeId}");
                } else {
                    Console.WriteLine("⚠ No se encontró el empleado.");
                }
            }

            var userClient = new Usuario {
                nombre = "TestClient1",
                primerApellido = "Apellido1",
                segundoApellido = "Apellido2",
                correo = "testclient@example.com",
                telefono = "1234567890",
                estadoUsuario = "Active"
            };

            int addResulClient = userOperation.addClient(userClient);
            Console.WriteLine($"Resultado creación cliente: {addResulClient}");

            using (var db = new MilYUnaNochesEntities()) {
                var client = db.Cliente.FirstOrDefault(c => c.Usuario.correo == "testclient@example.com");
                if (client != null) {
                    _clientId = client.idCliente;
                    Console.WriteLine($"Cliente insertado con ID: {_clientId}");
                } else {
                    Console.WriteLine("⚠ No se encontró el cliente.");
                }
            }

            var products = new List<Producto> {
                new Producto {
                    codigoProducto = "PROD-TEST-11",
                    nombreProducto = "Producto Prueba 11",
                    descripcion = "Descripción producto prueba 1",
                    categoria = "TEST",
                    cantidadStock = 100,
                    precioVenta = 50.00m,
                    precioCompra = 30.00m,
                    imagen = new byte[] { 0x20, 0x20 }
                },
                new Producto {
                    codigoProducto = "PROD-TEST-21",
                    nombreProducto = "Producto Prueba 21",
                    descripcion = "Descripción producto prueba 2",
                    categoria = "TEST",
                    cantidadStock = 50,
                    precioVenta = 75.00m,
                    precioCompra = 45.00m,
                    imagen = new byte[] { 0x20, 0x20 }
                }
            };

            foreach (var product in products) {
                if (ProductOperation.SaveProduct(product)) {
                    var savedProduct = ProductOperation.GetProductByCode(product.codigoProducto);
                    if (savedProduct != null) {
                        _productIds.Add(savedProduct.idProducto);
                        _productNames.Add(savedProduct.nombreProducto);
                        Console.WriteLine($"Producto insertado: {savedProduct.nombreProducto} (ID: {savedProduct.idProducto})");
                    }
                }
            }
        }

        private void CleanupTestEnvironment() {
            Console.WriteLine("Limpiando entorno de prueba...");

            using (var db = new MilYUnaNochesEntities()) {
                try {
                    var ventas = db.Venta.Where(v => v.idEmpleado == _employeeId || v.idCliente == _clientId).ToList();
                    foreach (var venta in ventas) {
                        var detalles = db.VentaProducto.Where(vp => vp.idVenta == venta.idVenta).ToList();
                        db.VentaProducto.RemoveRange(detalles);
                        db.Venta.Remove(venta);
                    }
                    db.SaveChanges();

                    Console.WriteLine("Ventas y detalles eliminados.");
                    var client = db.Cliente.FirstOrDefault(c => c.idCliente == _clientId);
                    if (client != null) {
                        var user = db.Usuario.Find(client.idUsuario);
                        db.Cliente.Remove(client);
                        if (user != null) db.Usuario.Remove(user);
                        db.SaveChanges();
                        Console.WriteLine("Cliente y usuario eliminados.");
                    }

                    var employee = db.Empleado.FirstOrDefault(e => e.idEmpleado == _employeeId);
                    if (employee != null) {
                        var access = db.Acceso.FirstOrDefault(a => a.idEmpleado == _employeeId);
                        if (access != null) db.Acceso.Remove(access);

                        var user = db.Usuario.Find(employee.idUsuario);
                        db.Empleado.Remove(employee);
                        if (user != null) db.Usuario.Remove(user);
                        db.SaveChanges();
                        Console.WriteLine("Empleado, acceso y usuario eliminados.");
                    }
                } catch (Exception ex) {
                    Console.WriteLine("❌ Error durante la limpieza: " + ex.Message);
                    if (ex.InnerException != null) {
                        Console.WriteLine("🔎 Inner: " + ex.InnerException.Message);
                    }
                    throw;
                }
            }

            foreach (var productName in _productNames) {
                try {
                    ProductOperation.DeleteProduct(productName);
                    Console.WriteLine($"Producto eliminado: {productName}");
                } catch (Exception ex) {
                    Console.WriteLine($"Error al eliminar producto {productName}: {ex.Message}");
                }
            }

            try {
                AddressOperation.DeleteAddress(_addressId);
            } catch {
            }
        }

        [Fact]
        public void ValidateStockTest_WithValidStock() {
            var saleDetails = new List<VentaProducto>
            {
                new VentaProducto { idProducto = _productIds[0], cantidadProducto = 10 },
                new VentaProducto { idProducto = _productIds[1], cantidadProducto = 5 }
            };
            var result = SaleOperation.ValidateStock(saleDetails);
            Assert.Equal(Constants.DataMatches, result);
        }

        [Fact]
        public void ValidateStockTest_InsufficientStock_ReturnsNoDataMatches() {
            var saleDetails = new List<VentaProducto>{
                new VentaProducto { idProducto = _productIds[0], cantidadProducto = 9999 }
            };
            var result = SaleOperation.ValidateStock(saleDetails);
            Assert.Equal(Constants.NoDataMatches, result);
        }

        [Fact]
        public void ValidateStockTest_ProductNotFound_ReturnsNoDataMatches() {
            var saleDetails = new List<VentaProducto>{
                new VentaProducto { idProducto = -1, cantidadProducto = 1 }
            };
            var result = SaleOperation.ValidateStock(saleDetails);
            Assert.Equal(Constants.NoDataMatches, result);
        }

        [Fact]
        public void ValidateStockTest_StockEqualToRequested_ReturnsDataMatches() {
            var saleDetails = new List<VentaProducto>{
                new VentaProducto { idProducto = _productIds[0], cantidadProducto = 100 }
            };
            var result = SaleOperation.ValidateStock(saleDetails);
            Assert.Equal(Constants.DataMatches, result);
        }

        [Fact]
        public void RegisterSaleTest_ValidSale() {
            var sale = new Venta {
                idEmpleado = _employeeId,
                idCliente = _clientId,
                fecha = DateTime.Today,
                hora = TimeSpan.FromHours(12),
                metodoPago = "EFECTIVO"
            };
            var details = new List<VentaProducto>
            {
                new VentaProducto { idProducto = _productIds[0], cantidadProducto = 2 },
                new VentaProducto { idProducto = _productIds[1], cantidadProducto = 1 }
            };
            var result = SaleOperation.RegisterSale(sale, details);
            Assert.Equal(Constants.SuccessOperation, result);
            using (var db = new MilYUnaNochesEntities()) {
                var venta = db.Venta.OrderByDescending(v => v.idVenta).FirstOrDefault();
                Assert.NotNull(venta);
                Assert.Equal(2 * 50.00m + 1 * 75.00m, venta.montoTotal);
                var detalles = db.VentaProducto.Where(vp => vp.idVenta == venta.idVenta).ToList();
                Assert.Equal(2, detalles.Count);
                var producto1 = db.Producto.Find(_productIds[0]);
                Assert.Equal(98, producto1.cantidadStock);
            }
        }

        [Fact]
        public void RegisterSaleTest_ProductNotFound_ShouldRollback() {
            var sale = new Venta {
                idEmpleado = _employeeId,
                idCliente = _clientId,
                fecha = DateTime.Today,
                hora = TimeSpan.FromHours(9),
                metodoPago = "TARJETA"
            };
            var details = new List<VentaProducto>{
                new VentaProducto { idProducto = -1, cantidadProducto = 1 }
            };
            var result = SaleOperation.RegisterSale(sale, details);
            Assert.Equal(Constants.NoDataMatches, result);
        }

        [Fact]
        public void RegisterSaleTest_InsufficientStock_ShouldRollback() {
            var sale = new Venta {
                idEmpleado = _employeeId,
                idCliente = _clientId,
                fecha = DateTime.Today,
                hora = TimeSpan.FromHours(11),
                metodoPago = "EFECTIVO"
            };
            var details = new List<VentaProducto>{
                new VentaProducto { idProducto = _productIds[0], cantidadProducto = 9999 }
            };
            var result = SaleOperation.RegisterSale(sale, details);
            Assert.Equal(Constants.ErrorOperation, result);
        }

        [Fact]
        public void RegisterSaleTest_NoClient_ShouldRollback() {
            var sale = new Venta {
                idEmpleado = _employeeId,
                fecha = DateTime.Today,
                hora = TimeSpan.FromHours(10),
                metodoPago = "EFECTIVO"
            };
            var details = new List<VentaProducto> {
                new VentaProducto { idProducto = _productIds[0], cantidadProducto = 1 }
            };
            var result = SaleOperation.RegisterSale(sale, details);
            Assert.Equal(Constants.SuccessOperation, result);
        }

        [Fact]
        public void RegisterSaleTest_NoEmployee_ShouldRollback() {
            var sale = new Venta {
                idEmpleado = -1,
                idCliente = _clientId,
                fecha = DateTime.Today,
                hora = TimeSpan.FromHours(12),
                metodoPago = "TARJETA"
            };
            var details = new List<VentaProducto> {
                new VentaProducto { idProducto = _productIds[0], cantidadProducto = 1 }
            };
            var result = SaleOperation.RegisterSale(sale, details);
            Assert.Equal(Constants.ErrorOperation, result);
        }

        [Fact]
        public void GetSalesTest_OnlyEmployeeFilter_NoSales() {
            var result = SaleOperation.GetSales(null, -999);
            Assert.Equal(Constants.NoDataMatches, result.ResultCode);
            Assert.Empty(result.Sales);
        }
    }
}