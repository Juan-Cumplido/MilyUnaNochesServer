using DataBaseManager;
using MilyUnaNochesService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Contracts {
    [ServiceContract]
    public interface IProductsManager {
        [OperationContract]
        bool SaveProduct(Product product);

        [OperationContract]
        List<Product> GetProducts();

        [OperationContract]
        bool ValidateProductName(string productName);

        [OperationContract]
        Task<Product> GetProductByCodeAsync(string productCode);
        
        [OperationContract]
        Task<bool> CheckStockByCodeAsync(string productCode, int quantity);
       
        [OperationContract]
        StockResponse GetProductStock(int productId);

        [OperationContract]
        bool UpdateProduct(Product product, string oldProductName);

        [OperationContract]
        bool DeleteProduct(string productName);

        [OperationContract]
        int NumProducts();
    }
    [DataContract]
    public class StockResponse {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public int Stock { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
