using MilyUnaNochesServer.Contracts;
using MilyUnaNochesService.Logic;
using DataBaseManager.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilyUnaNochesService.Contracts;
using DataBaseManager;

namespace MilyUnaNochesServer.Services {
    public partial class MilYUnaNochesService : IProviderManager {

        public int CreateProvider(Provider newProvider) {
            DataBaseManager.Proveedor provider = new DataBaseManager.Proveedor();
            int insertionResult = ProviderOperation.AddProvider(provider);
            return insertionResult;
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

        public int DeleteProvider(int idProvider) {
            int deletionResult = ProviderOperation.DeleteProvider(idProvider);
            return deletionResult;
        }

        public int ArchiveProvider(int idProvider) {
            int archiveResult = ProviderOperation.archiveProvider(idProvider);
            return archiveResult;
        }
    }
}
