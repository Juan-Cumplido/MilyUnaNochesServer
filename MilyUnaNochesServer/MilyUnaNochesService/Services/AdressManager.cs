using DataBaseManager;
using DataBaseManager.Operations;
using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Services {
    public partial class MilyUnaNochesService : IAdressManager {
        public int createAddress(Address address) {
            DataBaseManager.Direccion newAddress = new DataBaseManager.Direccion() {
                calle = address.Calle,
                numero = address.Numero,
                codigoPostal = address.CodigoPostal,
                ciudad = address.Ciudad
            };
            int insertionResult = AddressOperation.CreateAddress(newAddress);
            return insertionResult;
        }

        public Address GetAddress(int idDireccion) {
            Direccion direccion = AddressOperation.GetAddress(idDireccion);
            Address address = new Address() {
                Calle = direccion.calle,
                Numero = direccion.numero,
                Ciudad = direccion.ciudad,
                CodigoPostal = direccion.codigoPostal
            };
            return address; 
        }
    }
}
