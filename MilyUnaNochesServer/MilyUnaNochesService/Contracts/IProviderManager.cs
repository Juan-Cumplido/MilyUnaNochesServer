using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesServer.Contracts {
    [ServiceContract]
    public interface IProviderManager {
        [OperationContract]
        int CreateProvider(Provider newProvider);

        [OperationContract]
        List<Provider> GetProviders();

        [OperationContract]
        int ArchiveProvider(int idProvider);

        [OperationContract]
        int DeleteProvider(int idProvider);
    }
}
