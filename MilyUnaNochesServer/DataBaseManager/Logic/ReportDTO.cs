using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataBaseManager.Logic {
    #region DTOs principales
    [DataContract]
    public class ReportMetadata {
        [DataMember] public string ReportId { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public DateTime StartDate { get; set; }
        [DataMember] public DateTime EndDate { get; set; }
        [DataMember] public string PeriodType { get; set; }
        [DataMember] public int ItemCount { get; set; }
        [DataMember] public decimal TotalValue { get; set; }
        [DataMember] public string Category { get; set; }
    }

    [DataContract]
    public class ProductReportData {
        [DataMember] public string ReportId { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public DateTime StartDate { get; set; }
        [DataMember] public DateTime EndDate { get; set; }
        [DataMember] public ProductInfo ProductInfo { get; set; }
        [DataMember] public List<ProductSalesDetail> SalesDetails { get; set; }
        [DataMember] public SalesSummary Summary { get; set; }
    }

    [DataContract]
    public class CategoryReportData {
        [DataMember] public string ReportId { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public DateTime StartDate { get; set; }
        [DataMember] public DateTime EndDate { get; set; }
        [DataMember] public string CategoryName { get; set; }
        [DataMember] public List<CategorySalesDetail> SalesDetails { get; set; }
        [DataMember] public List<TopProduct> TopProducts { get; set; }
        [DataMember] public SalesSummary Summary { get; set; }
    }

    [DataContract]
    public class InventoryReportData {
        [DataMember] public string ReportId { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public DateTime ReportDate { get; set; }
        [DataMember] public List<InventoryItem> Items { get; set; }
        [DataMember] public InventorySummary Summary { get; set; }
    }

    [DataContract]
    public class ProfitReportData {
        [DataMember] public string ReportId { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public DateTime StartDate { get; set; }
        [DataMember] public DateTime EndDate { get; set; }
        [DataMember] public FinancialSummary Financials { get; set; }
        [DataMember] public List<CategoryProfit> CategoryBreakdown { get; set; }
        [DataMember] public List<MonthlyProfit> MonthlyTrend { get; set; }
    }
    #endregion

    #region DTOs detallados
    [DataContract]
    public class ProductInfo {
        [DataMember] public int ProductId { get; set; }
        [DataMember] public string ProductCode { get; set; }
        [DataMember] public string ProductName { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public string Category { get; set; }
        [DataMember] public decimal CurrentPrice { get; set; }
        [DataMember] public int CurrentStock { get; set; }
        [DataMember] public DateTime? LastRestockDate { get; set; }
    }

    [DataContract]
    public class ProductSalesDetail {
        [DataMember] public DateTime SaleDate { get; set; }
        [DataMember] public TimeSpan SaleTime { get; set; }
        [DataMember] public int QuantitySold { get; set; }
        [DataMember] public decimal UnitPrice { get; set; }
        [DataMember] public decimal CostPrice { get; set; }
        [DataMember] public string TransactionId { get; set; }
        [DataMember] public string SoldBy { get; set; }
        [DataMember] public string CustomerName { get; set; }
    }

    [DataContract]
    public class CategorySalesDetail {
        [DataMember] public DateTime SaleDate { get; set; }
        [DataMember] public int ProductsSold { get; set; }
        [DataMember] public decimal TotalSales { get; set; }
        [DataMember] public decimal TotalProfit { get; set; }
        [DataMember] public string TransactionId { get; set; }
    }

    [DataContract]
    public class TopProduct {
        [DataMember] public int ProductId { get; set; }
        [DataMember] public string ProductCode { get; set; }
        [DataMember] public string ProductName { get; set; }
        [DataMember] public int UnitsSold { get; set; }
        [DataMember] public decimal TotalSales { get; set; }
        [DataMember] public decimal Profit { get; set; }
    }

    [DataContract]
    public class InventoryItem {
        [DataMember] public int ProductId { get; set; }
        [DataMember] public string ProductCode { get; set; }
        [DataMember] public string ProductName { get; set; }
        [DataMember] public string Category { get; set; }
        [DataMember] public int CurrentStock { get; set; }
        [DataMember] public decimal PurchasePrice { get; set; }
        [DataMember] public decimal SalePrice { get; set; }
        [DataMember] public DateTime? LastRestockDate { get; set; }
        [DataMember] public DateTime? LastSaleDate { get; set; }
        [DataMember] public string SupplierName { get; set; }
    }
    #endregion

    #region DTOs de resumen
    [DataContract]
    public class SalesSummary {
        [DataMember] public int TotalTransactions { get; set; }
        [DataMember] public int TotalItemsSold { get; set; }
        [DataMember] public decimal TotalSales { get; set; }
        [DataMember] public decimal TotalProfit { get; set; }
        [DataMember] public decimal AverageSale { get; set; }
    }

    [DataContract]
    public class InventorySummary {
        [DataMember] public int TotalProducts { get; set; }
        [DataMember] public int OutOfStock { get; set; }
        [DataMember] public int LowStock { get; set; }
        [DataMember] public decimal TotalInventoryValue { get; set; }
    }

    [DataContract]
    public class FinancialSummary {
        [DataMember] public decimal TotalSales { get; set; }
        [DataMember] public decimal TotalCosts { get; set; }
        [DataMember] public decimal GrossProfit { get; set; }
        [DataMember] public decimal ProfitMargin { get; set; }
        [DataMember] public decimal OperatingExpenses { get; set; }
        [DataMember] public decimal NetProfit { get; set; }

    }

    [DataContract]
    public class CategoryProfit {
        [DataMember] public string CategoryName { get; set; }
        [DataMember] public decimal SalesPercentage { get; set; }
        [DataMember] public decimal ProfitPercentage { get; set; }
    }

    [DataContract]
    public class MonthlyProfit {
        [DataMember] public string MonthYear { get; set; }
        [DataMember] public decimal Sales { get; set; }
        [DataMember] public decimal Profit { get; set; }
    }
    #endregion
}