using DataBaseManager.Operations;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Tests.Utils;
using DataBaseManager;

namespace Tests.Operations {
    public class ProviderOperationTest : IClassFixture<DBFixtureProviderOperation> {
        private readonly DBFixtureProviderOperation fixture;
        public ProviderOperationTest(DBFixtureProviderOperation fixture) {
            this.fixture = fixture;
        }

        [Fact]
        public void AddProviderTest() {
            var provider = new Proveedor {
                nombreProveedor = "Test Provider",
                contacto = "Contacto Test",
                telefono = "1234567890",
                correo = "testprovider@example.com",
                idDireccion = fixture.Address100
            };

            int expectedResult = Constants.SuccessOperation;
            int obtainedResult = ProviderOperation.AddProvider(provider);

            Assert.Equal(expectedResult, obtainedResult);
        }

        [Fact]
        public void GetRegisteredProvidersTest() {

            List<Proveedor> providers = ProviderOperation.GetRegisteredProviders();

            Assert.True(providers.Count > 0);
        }

        [Fact]
        public void GetArchivedProvidersTest() {
            var provider = new Proveedor {
                nombreProveedor = "Test Archive Provider",
                contacto = "Contacto Archive",
                telefono = "1234567890",
                correo = "archive@example.com",
                idDireccion = fixture.Address101
            };

            int addResult = ProviderOperation.AddProvider(provider);
            Assert.Equal(Constants.SuccessOperation, addResult);

            List<Proveedor> registered = ProviderOperation.GetRegisteredProviders();
            Proveedor testProv = registered.FirstOrDefault(p => p.nombreProveedor == provider.nombreProveedor);
            Assert.NotNull(testProv);

            int archiveResult = ProviderOperation.archiveProvider(testProv.idProveedor);
            Assert.Equal(Constants.SuccessOperation, archiveResult);

            List<Proveedor> archived = ProviderOperation.GetArchivedProviders();
            bool found = archived.Any(p => p.idProveedor == testProv.idProveedor);
            Assert.True(found);
        }

        [Fact]
        public void UnArchiveProviderTest() {
            var provider = new Proveedor {
                nombreProveedor = "Test Unarchive Provider",
                contacto = "Contacto Unarchive",
                telefono = "1234567890",
                correo = "unarchive@example.com",
                idDireccion = fixture.Address102
            };

            int addResult = ProviderOperation.AddProvider(provider);
            Assert.Equal(Constants.SuccessOperation, addResult);

            List<Proveedor> registered = ProviderOperation.GetRegisteredProviders();
            Proveedor testProv = registered.FirstOrDefault(p => p.nombreProveedor == provider.nombreProveedor);
            Assert.NotNull(testProv);

            int archiveResult = ProviderOperation.archiveProvider(testProv.idProveedor);
            Assert.Equal(Constants.SuccessOperation, archiveResult);

            int unarchiveResult = ProviderOperation.UnArchiveProvider(testProv.idProveedor);
            Assert.Equal(Constants.SuccessOperation, unarchiveResult);
        }

        [Fact]
        public void DeleteProviderTest() {
            var provider = new Proveedor {
                nombreProveedor = "Test Delete Provider",
                contacto = "Contacto Delete",
                telefono = "1234567890",
                correo = "delete@example.com",
                idDireccion = fixture.Address103
            };

            int addResult = ProviderOperation.AddProvider(provider);
            Assert.Equal(Constants.SuccessOperation, addResult);

            List<Proveedor> registered = ProviderOperation.GetRegisteredProviders();
            Proveedor testProv = registered.FirstOrDefault(p => p.nombreProveedor == provider.nombreProveedor);
            Assert.NotNull(testProv);

            int deleteResult = ProviderOperation.DeleteProvider(testProv.idProveedor);
            Assert.Equal(Constants.SuccessOperation, deleteResult);

            int verifyResult = ProviderOperation.VerifyProviderInDataBase(provider.nombreProveedor);
            Assert.Equal(Constants.NoDataMatches, verifyResult);
        }

        [Fact]
        public void VerifyProviderInDataBaseTest() {
            var provider = new Proveedor {
                nombreProveedor = "Test Verify Provider",
                contacto = "Contacto Verify",
                telefono = "1234567890",
                correo = "verify@example.com",
                idDireccion = fixture.Address104
            };

            int addResult = ProviderOperation.AddProvider(provider);
            Assert.Equal(Constants.SuccessOperation, addResult);

            int verifyResult = ProviderOperation.VerifyProviderInDataBase(provider.nombreProveedor);
            Assert.Equal(Constants.DataMatches, verifyResult);

            int verifyNonExistent = ProviderOperation.VerifyProviderInDataBase("Non Existent Provider");
            Assert.Equal(Constants.NoDataMatches, verifyNonExistent);
        }
    }
}
