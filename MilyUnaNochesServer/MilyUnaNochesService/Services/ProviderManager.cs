using MilyUnaNochesServer.Contracts;
using MilyUnaNochesService.Logic;
using DataBaseManager.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesServer.Services {
    public partial class MilYUnaNochesService : IProviderManager {
        public int CreateProvider(Provider newProvider) {
            DataBaseManager.Proveedor provider = new DataBaseManager.Proveedor();
            int insertionResult = ProviderOperation.AddProvider(provider);
            return insertionResult;
        }
    }
}
