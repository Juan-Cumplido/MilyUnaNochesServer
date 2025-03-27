using DataBaseManager;
using MilyUnaNochesService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Contracts
{
    [ServiceContract]
    public interface IProductsManager
    {
        [OperationContract]
        bool SaveProduct(Product product);

        [OperationContract]
        List<Product> GetProducts();
      
        [OperationContract]
        bool ValidateProductName(string productName);
    }
}