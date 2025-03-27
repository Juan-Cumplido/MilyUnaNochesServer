using DataBaseManager;
using DataBaseManager.Operations;
using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Logic;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Services {
    public partial class MilyUnaNochesService : IProviderManager {
        public int ArchiveProvider(int idProvider) {
            int archiveResult = ProviderOperation.archiveProvider(idProvider);
            return archiveResult;
        }

        public int CreateProvider(Provider newProvider) {
            DataBaseManager.Proveedor provider = new DataBaseManager.Proveedor() {
                nombreProveedor = newProvider.providerName,
                contacto = newProvider.providerContact,
                telefono = newProvider.phoneNumber,
                correo = newProvider.email,
                idDireccion = newProvider.idAddress
            };
            int insertionResult = ProviderOperation.AddProvider(provider);
            return insertionResult;
        }

        public int DeleteProvider(int idProvider) {
            int deletionResult = ProviderOperation.DeleteProvider(idProvider);
            return deletionResult;
        }

        public List<Provider> GetArchivedProviders() {
            List<Proveedor> providersList = ProviderOperation.GetArchivedProviders();
            List<Provider> providers = new List<Provider>();
            foreach (Proveedor providerEntity in providersList) {

                Direccion addressData = AddressOperation.GetAddress(providerEntity.idDireccion);

                string addressText = addressData != null
                 ? $"{addressData.calle} {addressData.numero}, {addressData.ciudad}. CP: {addressData.codigoPostal}"
                : "Sin dirección";

                Provider provider = new Provider {
                    IdProvider = providerEntity.idProveedor,
                    providerName = providerEntity.nombreProveedor,
                    providerContact = providerEntity.contacto,
                    phoneNumber = providerEntity.telefono,
                    email = providerEntity.correo,
                    idAddress = providerEntity.idDireccion,
                    providerAddress = addressText,
                };
                providers.Add(provider);
            }
            return providers;
        }

        public List<Provider> GetProviders() {
            List<Proveedor> providersList = ProviderOperation.GetRegisteredProviders();
            List<Provider> providers = new List<Provider>();
            foreach (Proveedor providerEntity in providersList) {

                Direccion addressData = AddressOperation.GetAddress(providerEntity.idDireccion);

                string addressText = addressData != null
                 ? $"{addressData.calle} {addressData.numero}, {addressData.ciudad}. CP: {addressData.codigoPostal}"
                : "Sin dirección";

                Provider provider = new Provider {
                    IdProvider = providerEntity.idProveedor,
                    providerName = providerEntity.nombreProveedor,
                    providerContact = providerEntity.contacto,
                    phoneNumber = providerEntity.telefono,
                    email = providerEntity.correo,
                    idAddress = providerEntity.idDireccion,
                    providerAddress = addressText
                };
                providers.Add(provider);
            }
            return providers;
        }

        public int UnArchiveProvider(int idProvider) {
            int unarchiveResult = ProviderOperation.UnArchiveProvider(idProvider);
            return unarchiveResult;
        }

        public int VerifyProviderExistance(string providerName) {
            int providerExistance;
            providerExistance = ProviderOperation.VerifyProviderInDataBase(providerName);
            return providerExistance;
        }

        public Provider GetSupplier(int idProvider) {
            var query = ProviderOperation.GetSupplierInfo(idProvider);
            Provider provider = new Provider() {
                IdProvider = query.idProveedor,
                email = query.correo,
                providerContact = query.contacto,
                phoneNumber = query.telefono,
                providerName = query.nombreProveedor
            };
            return provider;
        }

        public int EditSupplier(Provider providerInfo, Address addressInfo) {
            int operationResult;
            Proveedor newProviderInfo = new Proveedor() {
                idProveedor = providerInfo.IdProvider,
                idDireccion = providerInfo.idAddress,
                nombreProveedor = providerInfo.providerName,
                contacto = providerInfo.providerContact,
                correo = providerInfo.email,
                telefono = providerInfo.phoneNumber
            };

            Direccion newAddressInfo = new Direccion() {
                calle = addressInfo.Calle,
                numero = addressInfo.Numero,
                codigoPostal = addressInfo.CodigoPostal,
                ciudad = addressInfo.Ciudad
            };
            operationResult = ProviderOperation.EditSupplierInfo(newProviderInfo, newAddressInfo);
            return operationResult;
        }
    }
}
