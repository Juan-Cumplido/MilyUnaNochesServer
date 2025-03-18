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

        public List<Provider> GetProviders() {
            List<Proveedor> providersList = ProviderOperation.GetRegisteredProviders();
            List<Provider> providers = new List<Provider>();
            foreach (Proveedor providerEntity in providersList) {
                Provider provider = new Provider {
                    IdProvider = providerEntity.idProveedor,
                    providerName = providerEntity.nombreProveedor,
                    providerContact = providerEntity.contacto,
                    phoneNumber = providerEntity.telefono,
                    email = providerEntity.correo
                };
                providers.Add(provider);
            }
            return providers;
        }
    }
}
