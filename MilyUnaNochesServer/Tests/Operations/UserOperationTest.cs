using DataBaseManager.Operations;
using DataBaseManager;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Tests.Utils;

namespace Tests.Operations {
    public class UserOperationTest : IClassFixture<DBFixtureUserOperation> {
        private readonly DBFixtureUserOperation fixture;
        private readonly UserOperation userOperation;

        public UserOperationTest(DBFixtureUserOperation fixture) {
            this.fixture = fixture;
            this.userOperation = new UserOperation();
        }

        [Fact]
        public void AddClientTest() {
            var user = new Usuario {
                nombre = "TestClient",
                primerApellido = "Apellido1",
                segundoApellido = "Apellido2",
                correo = "testclient@example.com",
                telefono = "1234567890",
                estadoUsuario = "Active"
            };

            int addResult = userOperation.addClient(user);
            Assert.Equal(Constants.SuccessOperation, addResult);

            int verifyResult = userOperation.VerifyExistingClientIntoDataBase(user.nombre, user.primerApellido, user.segundoApellido);
            Assert.Equal(Constants.DataMatches, verifyResult);
        }

        [Fact]
        public void AddEmployeeTest() {
            var user = new Usuario {
                nombre = "TestEmployee",
                primerApellido = "EmpleadoApellido1",
                segundoApellido = "EmpleadoApellido2",
                correo = "testemployee@example.com",
                telefono = "0987654321",
                estadoUsuario = "Active"
            };

            var address = new Direccion {
                calle = "Test Calle Emp",
                numero = "10",
                codigoPostal = "CP10",
                ciudad = "Test Ciudad"
            };

            var employee = new Empleado {
                tipoEmpleado = "Staff"
            };

            var access = new Acceso {
                usuario = "EmpUser",
                contraseña = "EmpPass"
            };

            int addResult = userOperation.addEmployee(user, address, employee, access);
            Assert.Equal(Constants.SuccessOperation, addResult);

            int verifyResult = userOperation.VerifyExistingEmployeeIntoDataBase(user.correo, user.telefono);
            Assert.Equal(Constants.DataMatches, verifyResult);
        }

        [Fact]
        public void GetClientsByNameOrPhoneTest() {
            var user = new Usuario {
                nombre = "TestSearchClient",
                primerApellido = "BuscarApellido",
                segundoApellido = "BuscarApellido2",
                correo = "searchclient@example.com",
                telefono = "1112223333",
                estadoUsuario = "Active"
            };

            int addResult = userOperation.addClient(user);
            Assert.Equal(Constants.SuccessOperation, addResult);

            List<ClientData> clientsFound = userOperation.GetClientsByNameOrPhone("TestSearch");
            Assert.True(clientsFound.Any(c => c.idUsuario > 0));  

            List<ClientData> clientsFoundByPhone = userOperation.GetClientsByNameOrPhone("111222");
            Assert.True(clientsFoundByPhone.Any(c => c.idUsuario > 0));
        }

        [Fact]
        public void GetActiveEmployeesBySearchTermTest() {
            var user = new Usuario {
                nombre = "TestSearchEmployee",
                primerApellido = "EmpleadoApellido",
                segundoApellido = "EmpleadoApellido2",
                correo = "searchemployee@example.com",
                telefono = "4445556666",
                estadoUsuario = "Active"
            };

            var address = new Direccion {
                calle = "Calle Empleado",
                numero = "20",
                codigoPostal = "CP20",
                ciudad = "CiudadEmpleado"
            };

            var employee = new Empleado {
                tipoEmpleado = "Staff"
            };

            var access = new Acceso {
                usuario = "SearchEmpUser",
                contraseña = "SearchEmpPass"
            };

            int addResult = userOperation.addEmployee(user, address, employee, access);
            Assert.Equal(Constants.SuccessOperation, addResult);

            List<EmployeeData> employeesFound = userOperation.GetActiveEmployeesBySearchTerm("TestSearchEmployee");
            Assert.True(employeesFound.Any(e => e.idUsuario > 0));
        }

        [Fact]
        public void ArchiveClientTest() {
            var user = new Usuario {
                nombre = "TestArchiveClient",
                primerApellido = "ArchApellido1",
                segundoApellido = "ArchApellido2",
                correo = "archiveclient@example.com",
                telefono = "7778889999",
                estadoUsuario = "Active"
            };

            int addResult = userOperation.addClient(user);
            Assert.Equal(Constants.SuccessOperation, addResult);

            int verifyBefore = userOperation.VerifyExistingClientIntoDataBase(user.nombre, user.primerApellido, user.segundoApellido);
            Assert.Equal(Constants.DataMatches, verifyBefore);

            int archiveResult = userOperation.ArchiveClient(fixture.GetUserIdByName(user.nombre));
            Assert.Equal(Constants.DataMatches, archiveResult);
        }

        [Fact]
        public void ArchiveEmployeeTest() {
            var user = new Usuario {
                nombre = "TestArchiveEmployee",
                primerApellido = "EmpArchApellido1",
                segundoApellido = "EmpArchApellido2",
                correo = "archiveemployee@example.com",
                telefono = "3334445555",
                estadoUsuario = "Active"
            };

            var address = new Direccion {
                calle = "Calle Emp Archive",
                numero = "30",
                codigoPostal = "CP30",
                ciudad = "CiudadEmpArchive"
            };

            var employee = new Empleado {
                tipoEmpleado = "Manager"
            };

            var access = new Acceso {
                usuario = "EmpArchUser",
                contraseña = "EmpArchPass"
            };

            int addResult = userOperation.addEmployee(user, address, employee, access);
            Assert.Equal(Constants.SuccessOperation, addResult);

            int verifyBefore = userOperation.VerifyExistingEmployeeIntoDataBase(user.correo, user.telefono);
            Assert.Equal(Constants.DataMatches, verifyBefore);

            int archiveResult = userOperation.ArchiveEmployee(fixture.GetUserIdByName(user.nombre));
            Assert.Equal(Constants.DataMatches, archiveResult);
        }
    }
}