using DataBaseManager.Operations;
using DataBaseManager;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Operations {
    public class AddressOperationTest : IDisposable {
        private int createdAddressId;

        [Fact]
        public void CreateAddressTest() {
            var address = new Direccion {
                calle = "Test Calle",
                numero = "123",
                codigoPostal = "CP123",
                ciudad = "Test Ciudad"
            };

            int idCreated = AddressOperation.CreateAddress(address);

            Assert.NotEqual(Constants.ErrorOperation, idCreated);
            createdAddressId = idCreated;
        }


        [Fact]
        public void DeleteAddressTest() {
            var address = new Direccion {
                calle = "Test Calle Delete",
                numero = "789",
                codigoPostal = "CP789",
                ciudad = "Test Ciudad"
            };
            int idCreated = AddressOperation.CreateAddress(address);
            Assert.NotEqual(Constants.ErrorOperation, idCreated);

            bool isDeleted = AddressOperation.DeleteAddress(idCreated);

            Assert.True(isDeleted);

            var fetchedAddress = AddressOperation.GetAddress(idCreated);
            Assert.Equal(Constants.NoDataMatches, fetchedAddress.idDireccion);
        }

        public void Dispose() {
            if (createdAddressId != 0) {
                AddressOperation.DeleteAddress(createdAddressId);
            }
        }
    }
}