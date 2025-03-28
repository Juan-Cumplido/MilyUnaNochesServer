using DataBaseManager;
using MilyUnaNochesService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Contracts {
    [ServiceContract]
    public interface IProviderManager {
        [OperationContract]
        int CreateProvider(Provider newProvider);

        [OperationContract]
        List<Provider> GetProviders();

        [OperationContract]
        List<Provider> GetArchivedProviders();

        [OperationContract]
        int ArchiveProvider(int idProvider);

        [OperationContract]
        int DeleteProvider(int idProvider);

        [OperationContract]
        int VerifyProviderExistance(string providerName);

        [OperationContract]
        int UnArchiveProvider(int idProvider);
        
        [OperationContract]
        Provider GetSupplier(int idProvider);
        [OperationContract]
        int EditSupplier(Provider newProviderInfo, Address newAddressInfo);
    }
}
