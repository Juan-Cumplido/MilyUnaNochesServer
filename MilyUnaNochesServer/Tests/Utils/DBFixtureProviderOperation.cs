using DataBaseManager;
using DataBaseManager.Operations;
using MilyUnaNochesService.Utilities;
using System;
using System.Linq;

namespace Tests.Utils {
    public class DBFixtureProviderOperation : IDisposable {

        public int Address100 { get; private set; }
        public int Address101 { get; private set; }
        public int Address102 { get; private set; }
        public int Address103 { get; private set; }
        public int Address104 { get; private set; }
        public int Address105 { get; private set; }

        public DBFixtureProviderOperation() {
            Address100 = CreateAddress("Test Calle 100", "100", "CP100", "Test Ciudad");
            Address101 = CreateAddress("Test Calle 101", "101", "CP101", "Test Ciudad");
            Address102 = CreateAddress("Test Calle 102", "102", "CP102", "Test Ciudad");
            Address103 = CreateAddress("Test Calle 103", "103", "CP103", "Test Ciudad");
            Address104 = CreateAddress("Test Calle 104", "104", "CP104", "Test Ciudad");
            Address104 = CreateAddress("Test Calle 104", "104", "CP104", "Test Ciudad");
        }

        private int CreateAddress(string calle, string numero, string codigoPostal, string ciudad) {
            var direccion = new Direccion {
                calle = calle,
                numero = numero,
                codigoPostal = codigoPostal,
                ciudad = ciudad
            };

            int idCreated = AddressOperation.CreateAddress(direccion);
            return idCreated;
        }

        public void Dispose() {

            var registeredProviders = ProviderOperation.GetRegisteredProviders()
                .Where(p => p.nombreProveedor.StartsWith("Test"))
                .ToList();

            foreach (var provider in registeredProviders) {
                ProviderOperation.DeleteProvider(provider.idProveedor);
            }

            var archivedProviders = ProviderOperation.GetArchivedProviders()
                .Where(p => p.nombreProveedor.StartsWith("Test"))
                .ToList();

            foreach (var provider in archivedProviders) {
                ProviderOperation.DeleteProvider(provider.idProveedor);
            }

            AddressOperation.DeleteAddress(Address100);
            AddressOperation.DeleteAddress(Address101);
            AddressOperation.DeleteAddress(Address102);
            AddressOperation.DeleteAddress(Address103);
            AddressOperation.DeleteAddress(Address104);
        }
    }
}
