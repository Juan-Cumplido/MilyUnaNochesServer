using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using DataBaseManager.Logic;
using System;

namespace MilyUnaNochesService.Contracts {

    [ServiceContract]
    public interface IReportManager {
       
        [OperationContract]
        Task<List<ReportMetadata>> GetAvailableProductReportsAsync(string productCode, string periodType, int skip = 0, int take = 5);

        [OperationContract]
        Task<List<ReportMetadata>> GetAvailableCategoryReportsAsync(string category, string periodType, int skip = 0, int take = 5);

        [OperationContract]
        Task<List<ReportMetadata>> GetAvailableInventoryReportsAsync(string periodType, int skip = 0, int take = 5);

        [OperationContract]
        Task<List<ReportMetadata>> GetAvailableProfitReportsAsync(string periodType, int skip = 0, int take = 5);
        
        [OperationContract]
        Task<ProductReportData> GetProductReportDataAsync(string reportId);

        [OperationContract]
        Task<CategoryReportData> GetCategoryReportDataAsync(string reportId);

        [OperationContract]
        Task<InventoryReportData> GetInventoryReportDataAsync(string reportId);

        [OperationContract]
        Task<ProfitReportData> GetProfitReportDataAsync(string reportId);
        

       
        [OperationContract]
        Task<int> GetProductReportsCountAsync(string productCode, string periodType);

        [OperationContract]
        Task<int> GetCategoryReportsCountAsync(string category, string periodType);

        [OperationContract]
        Task<int> GetInventoryReportsCountAsync(string periodType);

        [OperationContract]
        Task<int> GetProfitReportsCountAsync(string periodType);
        
        [OperationContract]
        Task<List<ProductSalesDetail>> GetProductSalesDetailsAsync(string productCode, DateTime startDate, DateTime endDate);

        [OperationContract]
        Task<List<CategorySalesDetail>> GetCategorySalesDetailsAsync(string category, DateTime startDate, DateTime endDate);
       
    }

}