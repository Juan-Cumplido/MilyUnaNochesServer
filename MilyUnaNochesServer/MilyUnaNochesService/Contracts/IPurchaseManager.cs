using MilyUnaNochesService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Contracts {
    [ServiceContract]
    public interface IPurchaseManager {
        [OperationContract]
        int SavePurchase(RegisterPurchase_sv purchase);
    }
}
