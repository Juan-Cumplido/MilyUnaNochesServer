using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using DataBaseManager.Logic;
using DataBaseManager.Operations;
using System.Linq;
using MilyUnaNochesService.Contracts;

namespace MilyUnaNochesService.Services {
    public partial class MilyUnaNochesService : IReportManager {
        private readonly ReportOperations _reportOperations = new ReportOperations();

        #region Métodos para obtener metadatos de reportes (paginados)
        public async Task<List<ReportMetadata>> GetAvailableProductReportsAsync(string productCode, string periodType, int skip = 0, int take = 5) {
            return await Task.Run(() =>
            {
                try {
                    DateTime earliestDate;
                    try {
                        earliestDate = _reportOperations.GetEarliestProductDate(productCode);
                    } catch (InvalidOperationException) {
                        return new List<ReportMetadata>();
                    }

                    var dateRanges = _reportOperations
                        .GetDateRanges(periodType, skip + take, earliestDate)
                        .Skip(skip)
                        .Take(take)
                        .ToList(); // MATERIALIZAR

                    var result = new List<ReportMetadata>();

                    foreach (var range in dateRanges) {
                        // MATERIALIZAR explícitamente
                        var data = _reportOperations
                            .GetProductSalesData(productCode, range.StartDate, range.EndDate)
                            .ToList();

                        // MATERIALIZAR explícitamente
                        var productInfo = _reportOperations.GetProductInfo(productCode);

                        // Extraer valores simples para evitar interpolación en LINQ to Entities
                        string productName = productInfo?.ProductName ?? productCode;
                        string category = productInfo?.Category ?? "General";

                        result.Add(new ReportMetadata {
                            ReportId = $"PROD_{productCode}_{range.StartDate:yyyyMMdd}_{range.EndDate:yyyyMMdd}",
                            Title = $"Ventas de {productName}",
                            Description = $"Producto: {productCode}",
                            StartDate = range.StartDate,
                            EndDate = range.EndDate,
                            PeriodType = periodType,
                            ItemCount = data.Sum(d => d.QuantitySold),
                            TotalValue = data.Sum(d => d.QuantitySold * d.UnitPrice),
                            Category = category
                        });
                    }

                    return result;
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo metadatos de reportes de producto: {ex.StackTrace}");
                }
            });
        }


        public async Task<List<ReportMetadata>> GetAvailableCategoryReportsAsync(string category, string periodType, int skip = 0, int take = 5) {
            return await Task.Run(() => {
                try {
                    DateTime earliestDate;
                    try {
                        earliestDate = string.IsNullOrEmpty(category)
                            ? _reportOperations.GetEarliestSystemDate()
                            : _reportOperations.GetEarliestCategoryDate(category);
                    } catch (InvalidOperationException) {
                        return new List<ReportMetadata>();
                    }

                    var dateRanges = _reportOperations.GetDateRanges(periodType, skip + take, earliestDate)
                                    .Skip(skip).Take(take);
                    var result = new List<ReportMetadata>();

                    foreach (var range in dateRanges) {
                        var data = _reportOperations.GetCategorySalesData(category, range.StartDate, range.EndDate);
                        result.Add(new ReportMetadata {
                            ReportId = $"CAT_{category}_{range.StartDate:yyyyMMdd}_{range.EndDate:yyyyMMdd}",
                            Title = $"Ventas de categoría {category}",
                            Description = string.IsNullOrEmpty(category) ?
                                "Todos los productos" :
                                $"Productos de la categoría {category}",
                            StartDate = range.StartDate,
                            EndDate = range.EndDate,
                            PeriodType = periodType,
                            ItemCount = data.Sum(d => d.ProductsSold), // Usamos ProductsSold que sí existe
                            TotalValue = data.Sum(d => d.TotalSales),
                            Category = string.IsNullOrEmpty(category) ? "Todas las categorías" : category
                        });
                    }

                    return result;
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo metadatos de reportes por categoría: {ex.Message}");
                }
            });
        }

        public async Task<List<ReportMetadata>> GetAvailableInventoryReportsAsync(string periodType, int skip = 0, int take = 5) {
            return await Task.Run(() => {
                try {
                    return new List<ReportMetadata>
                    {
                        new ReportMetadata
                        {
                            ReportId = $"INV_{DateTime.Now:yyyyMMdd}",
                            Title = "Reporte de Inventario Actual",
                            Description = "Todos los productos en inventario",
                            StartDate = DateTime.Now.Date,
                            EndDate = DateTime.Now.Date,
                            PeriodType = "instantáneo",
                            ItemCount = _reportOperations.GetInventoryData().Count,
                            TotalValue = _reportOperations.GetInventoryData().Sum(d => d.PurchasePrice * d.CurrentStock),
                            Category = "Todas las categorías"
                        }
                    };
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo metadatos de reportes de inventario: {ex.Message}");
                }
            });
        }

        public async Task<List<ReportMetadata>> GetAvailableProfitReportsAsync(string periodType, int skip = 0, int take = 5) {
            return await Task.Run(() => {
                try {
                    DateTime earliestDate;
                    try {
                        earliestDate = _reportOperations.GetEarliestSystemDate();
                    } catch (InvalidOperationException) {
                        return new List<ReportMetadata>();
                    }

                    var dateRanges = _reportOperations.GetDateRanges(periodType, skip + take, earliestDate)
                                    .Skip(skip).Take(take);
                    var result = new List<ReportMetadata>();

                    foreach (var range in dateRanges) {
                        var data = _reportOperations.GetProfitData(range.StartDate, range.EndDate);
                        result.Add(new ReportMetadata {
                            ReportId = $"PROFIT_{range.StartDate:yyyyMMdd}_{range.EndDate:yyyyMMdd}",
                            Title = "Reporte de Ganancias",
                            Description = "Todos los productos con ventas",
                            StartDate = range.StartDate,
                            EndDate = range.EndDate,
                            PeriodType = periodType,
                            ItemCount = (int)data.Financials.TotalSales, // Usamos TotalSales como valor numérico
                            TotalValue = data.Financials.GrossProfit,
                            Category = "Todas las categorías"
                        });
                    }

                    return result;
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo metadatos de reportes de ganancias: {ex.Message}");
                }
            });
        }
        #endregion

        #region Métodos para obtener datos completos de reportes
        public async Task<ProductReportData> GetProductReportDataAsync(string reportId) {
            return await Task.Run(() => {
                try {
                    var parts = reportId.Split('_');
                    string productCode = parts[1];
                    DateTime startDate = DateTime.ParseExact(parts[2], "yyyyMMdd", null);
                    DateTime endDate = DateTime.ParseExact(parts[3], "yyyyMMdd", null);

                    var salesDetails = _reportOperations.GetProductSalesData(productCode, startDate, endDate);
                    var productInfo = _reportOperations.GetProductInfo(productCode);

                    return new ProductReportData {
                        ReportId = reportId,
                        Title = $"Ventas de {productInfo?.ProductName ?? productCode}",
                        StartDate = startDate,
                        EndDate = endDate,
                        ProductInfo = new ProductInfo {
                            ProductId = productInfo.ProductId,
                            ProductCode = productInfo.ProductCode,
                            ProductName = productInfo.ProductName,
                            Description = productInfo.Description,
                            Category = productInfo.Category,
                            CurrentPrice = productInfo.CurrentPrice,
                            CurrentStock = productInfo.CurrentStock,
                            LastRestockDate = productInfo.LastRestockDate
                        },
                        SalesDetails = salesDetails.Select(s => new ProductSalesDetail {
                            SaleDate = s.SaleDate,
                            SaleTime = s.SaleTime,
                            QuantitySold = s.QuantitySold,
                            UnitPrice = s.UnitPrice,
                            CostPrice = s.CostPrice,
                            TransactionId = s.TransactionId,
                            SoldBy = s.SoldBy,
                            CustomerName = s.CustomerName
                        }).ToList(),
                        Summary = new SalesSummary {
                            TotalTransactions = salesDetails.Select(s => s.TransactionId).Distinct().Count(),
                            TotalItemsSold = salesDetails.Sum(s => s.QuantitySold),
                            TotalSales = salesDetails.Sum(s => s.QuantitySold * s.UnitPrice),
                            TotalProfit = salesDetails.Sum(s => s.QuantitySold * (s.UnitPrice - s.CostPrice)),
                            AverageSale = salesDetails.Any() ? salesDetails.Average(s => s.QuantitySold * s.UnitPrice) : 0
                        }
                    };
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo datos de reporte de producto: {ex.Message}");
                }
            });
        }

        public async Task<CategoryReportData> GetCategoryReportDataAsync(string reportId) {
            return await Task.Run(() => {
                try {
                    var parts = reportId.Split('_');
                    string category = parts[1];
                    DateTime startDate = DateTime.ParseExact(parts[2], "yyyyMMdd", null);
                    DateTime endDate = DateTime.ParseExact(parts[3], "yyyyMMdd", null);

                    var salesDetails = _reportOperations.GetCategorySalesData(category, startDate, endDate);
                    var topProducts = _reportOperations.GetTopProductsByCategory(category, startDate, endDate, 5);

                    return new CategoryReportData {
                        ReportId = reportId,
                        Title = $"Ventas de categoría {category}",
                        StartDate = startDate,
                        EndDate = endDate,
                        CategoryName = category,
                        SalesDetails = salesDetails,
                        TopProducts = topProducts,
                        Summary = new SalesSummary {
                            TotalTransactions = salesDetails.Count, // Usamos el conteo de detalles
                            TotalItemsSold = salesDetails.Sum(s => s.ProductsSold),
                            TotalSales = salesDetails.Sum(s => s.TotalSales),
                            TotalProfit = salesDetails.Sum(s => s.TotalProfit),
                            AverageSale = salesDetails.Any() ? salesDetails.Average(s => s.TotalSales) : 0
                        }
                    };
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo datos de reporte por categoría: {ex.Message}");
                }
            });
        }

        public async Task<InventoryReportData> GetInventoryReportDataAsync(string reportId) {
            return await Task.Run(() => {
                try {
                    var inventoryItems = _reportOperations.GetInventoryData();

                    return new InventoryReportData {
                        ReportId = reportId,
                        Title = "Reporte de Inventario Actual",
                        ReportDate = DateTime.Now,
                        Items = inventoryItems.Select(i => new InventoryItem {
                            ProductId = i.ProductId,
                            ProductCode = i.ProductCode,
                            ProductName = i.ProductName,
                            Category = i.Category,
                            CurrentStock = i.CurrentStock,
                            PurchasePrice = i.PurchasePrice,
                            SalePrice = i.SalePrice,
                            LastRestockDate = i.LastRestockDate,
                            LastSaleDate = i.LastSaleDate,
                            SupplierName = i.SupplierName
                        }).ToList(),
                        Summary = new InventorySummary {
                            TotalProducts = inventoryItems.Count,
                            OutOfStock = inventoryItems.Count(i => i.CurrentStock <= 0),
                            LowStock = inventoryItems.Count(i => i.CurrentStock > 0 && i.CurrentStock < 10),
                            TotalInventoryValue = inventoryItems.Sum(i => i.PurchasePrice * i.CurrentStock)
                        }
                    };
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo datos de inventario: {ex.Message}");
                }
            });
        }

        public async Task<ProfitReportData> GetProfitReportDataAsync(string reportId) {
            return await Task.Run(() => {
                try {
                    var parts = reportId.Split('_');
                    DateTime startDate = DateTime.ParseExact(parts[1], "yyyyMMdd", null);
                    DateTime endDate = DateTime.ParseExact(parts[2], "yyyyMMdd", null);

                    var profitData = _reportOperations.GetProfitData(startDate, endDate);
                    var categoryBreakdown = _reportOperations.GetCategoryProfitBreakdown(startDate, endDate);
                    var monthlyTrend = _reportOperations.GetMonthlyProfitTrend(startDate, endDate);

                    return new ProfitReportData {
                        ReportId = reportId,
                        Title = "Reporte de Ganancias",
                        StartDate = startDate,
                        EndDate = endDate,
                        Financials = new FinancialSummary {
                            TotalSales = profitData.Financials.TotalSales,
                            TotalCosts = profitData.Financials.TotalCosts,
                            GrossProfit = profitData.Financials.GrossProfit,
                            ProfitMargin = profitData.Financials.ProfitMargin,
                            OperatingExpenses = profitData.Financials.OperatingExpenses,
                            NetProfit = profitData.Financials.NetProfit
                        },
                        CategoryBreakdown = categoryBreakdown,
                        MonthlyTrend = monthlyTrend
                    };
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo datos de ganancias: {ex.Message}");
                }
            });
        }
        #endregion

        #region Métodos auxiliares para paginación
        public async Task<int> GetProductReportsCountAsync(string productCode, string periodType) {
            return await Task.Run(() => {
                try {
                    DateTime earliestDate;
                    try {
                        earliestDate = _reportOperations.GetEarliestProductDate(productCode);
                    } catch (InvalidOperationException) {
                        return 0;
                    }

                    return _reportOperations.GetDateRanges(periodType, int.MaxValue, earliestDate).Count;
                } catch (Exception ex) {
                    throw new FaultException($"Error contando reportes de producto: {ex.Message}");
                }
            });
        }

        public async Task<int> GetCategoryReportsCountAsync(string category, string periodType) {
            return await Task.Run(() => {
                try {
                    DateTime earliestDate;
                    try {
                        earliestDate = string.IsNullOrEmpty(category)
                            ? _reportOperations.GetEarliestSystemDate()
                            : _reportOperations.GetEarliestCategoryDate(category);
                    } catch (InvalidOperationException) {
                        return 0;
                    }

                    return _reportOperations.GetDateRanges(periodType, int.MaxValue, earliestDate).Count;
                } catch (Exception ex) {
                    throw new FaultException($"Error contando reportes por categoría: {ex.Message}");
                }
            });
        }

        public async Task<int> GetInventoryReportsCountAsync(string periodType) {
            return await Task.Run(() => 1); // Siempre hay solo 1 reporte de inventario
        }

        public async Task<int> GetProfitReportsCountAsync(string periodType) {
            return await Task.Run(() => {
                try {
                    DateTime earliestDate;
                    try {
                        earliestDate = _reportOperations.GetEarliestSystemDate();
                    } catch (InvalidOperationException) {
                        return 0;
                    }

                    return _reportOperations.GetDateRanges(periodType, int.MaxValue, earliestDate).Count;
                } catch (Exception ex) {
                    throw new FaultException($"Error contando reportes de ganancias: {ex.Message}");
                }
            });
        }
        #endregion

        #region Métodos adicionales para flexibilidad
        public async Task<List<ProductSalesDetail>> GetProductSalesDetailsAsync(string productCode, DateTime startDate, DateTime endDate) {
            return await Task.Run(() => {
                try {
                    var salesData = _reportOperations.GetProductSalesData(productCode, startDate, endDate);
                    return salesData.Select(s => new ProductSalesDetail {
                        SaleDate = s.SaleDate,
                        SaleTime = s.SaleTime,
                        QuantitySold = s.QuantitySold,
                        UnitPrice = s.UnitPrice,
                        CostPrice = s.CostPrice,
                        TransactionId = s.TransactionId,
                        SoldBy = s.SoldBy,
                        CustomerName = s.CustomerName
                    }).ToList();
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo detalles de ventas de producto: {ex.Message}");
                }
            });
        }

        public async Task<List<CategorySalesDetail>> GetCategorySalesDetailsAsync(string category, DateTime startDate, DateTime endDate) {
            return await Task.Run(() => {
                try {
                    var salesData = _reportOperations.GetCategorySalesData(category, startDate, endDate);
                    return salesData.Select(s => new CategorySalesDetail {
                        SaleDate = s.SaleDate,
                        ProductsSold = s.ProductsSold,
                        TotalSales = s.TotalSales,
                        TotalProfit = s.TotalProfit,
                        TransactionId = s.TransactionId
                    }).ToList();
                } catch (Exception ex) {
                    throw new FaultException($"Error obteniendo detalles de ventas por categoría: {ex.Message}");
                }
            });
        }
        #endregion

        // Eliminar los métodos de generación de PDF (GenerateProductReport, GenerateCategoryReport, etc.)
        // Eliminar el método DownloadReportAsync
    }
}