using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DataBaseManager.Logic;

namespace DataBaseManager.Operations {
    public class ReportOperations {
        #region Métodos para Productos

        public List<ProductSalesDetail> GetProductSalesData(string productCode, DateTime startDate, DateTime endDate) {
            using (var context = new MilYUnaNochesEntities()) {
                // Consulta optimizada sin interpolación de strings
                var query = context.VentaProducto
                    .Where(vp => vp.Producto.codigoProducto == productCode &&
                                vp.Venta.fecha >= startDate &&
                                vp.Venta.fecha <= endDate)
                    .Select(vp => new {
                        vp.Venta.fecha,
                        vp.cantidadProducto,
                        vp.precioVentaHistorico,
                        // Datos del empleado (concatenación simple)
                        EmpleadoNombre = vp.Venta.Empleado.Usuario.nombre,
                        EmpleadoApellido = vp.Venta.Empleado.Usuario.primerApellido,
                        // Datos del cliente (manejo de nulls)
                        ClienteNombre = vp.Venta.Cliente != null ? vp.Venta.Cliente.Usuario.nombre : null,
                        ClienteApellido = vp.Venta.Cliente != null ? vp.Venta.Cliente.Usuario.primerApellido : null
                    })
                    .AsEnumerable()  // Ejecuta la consulta SQL aquí
                    .Select(x => new ProductSalesDetail {
                        SaleDate = x.fecha,
                        QuantitySold = x.cantidadProducto,
                        UnitPrice = x.precioVentaHistorico,
                        // Concatenación en memoria
                        SoldBy = x.EmpleadoNombre + " " + x.EmpleadoApellido,
                        CustomerName = x.ClienteNombre != null ?
                            x.ClienteNombre + " " + x.ClienteApellido :
                            "Venta general"
                    })
                    .ToList();

                return query;
            }
        }

        public ProductInfo GetProductInfo(string productCode) {
            using (var context = new MilYUnaNochesEntities()) {
                var product = context.Producto
                    .Where(p => p.codigoProducto == productCode)
                    .Select(p => new ProductInfo {
                        ProductId = p.idProducto,
                        ProductCode = p.codigoProducto,
                        ProductName = p.nombreProducto,
                        Description = p.descripcion,
                        Category = p.categoria,
                        CurrentPrice = p.precioVenta,
                        CurrentStock = p.cantidadStock,
                        LastRestockDate = p.CompraProducto.OrderByDescending(cp => cp.CompraProveedor.fecha)
                                                       .Select(cp => (DateTime?)cp.CompraProveedor.fecha)
                                                       .FirstOrDefault()
                    })
                    .FirstOrDefault();

                return product;
            }
        }

        public DateTime GetEarliestProductDate(string productCode) {
            using (var context = new MilYUnaNochesEntities()) {
                var earliestDate = context.VentaProducto
                    .Where(vp => vp.Producto.codigoProducto == productCode)
                    .Min(vp => vp.Venta.fecha);

                return earliestDate;
            }
        }
        #endregion

        #region Métodos para Categorías
        public List<CategorySalesDetail> GetCategorySalesData(string category, DateTime startDate, DateTime endDate) {
            using (var context = new MilYUnaNochesEntities()) {
                return context.VentaProducto
                    .Include(vp => vp.Producto)
                    .Include(vp => vp.Venta)
                    .Where(vp => (string.IsNullOrEmpty(category) || vp.Producto.categoria == category) &&
                                vp.Venta.fecha >= startDate &&
                                vp.Venta.fecha <= endDate)
                    .GroupBy(vp => new {
                        vp.Venta.fecha,
                        vp.idVenta
                    })
                    .Select(g => new CategorySalesDetail {
                        SaleDate = g.Key.fecha,
                        ProductsSold = g.Sum(x => x.cantidadProducto),
                        TotalSales = g.Sum(x => x.cantidadProducto * x.precioVentaHistorico),
                        TotalProfit = g.Sum(x => (x.precioVentaHistorico - x.precioCompraHistorico) * x.cantidadProducto),
                        TransactionId = g.Key.idVenta.ToString()
                    })
                    .ToList();
            }
        }

        public List<TopProduct> GetTopProductsByCategory(string category, DateTime startDate, DateTime endDate, int count) {
            using (var context = new MilYUnaNochesEntities()) {
                return context.VentaProducto
                    .Include(vp => vp.Producto)
                    .Include(vp => vp.Venta)
                    .Where(vp => (string.IsNullOrEmpty(category) || vp.Producto.categoria == category) &&
                                vp.Venta.fecha >= startDate &&
                                vp.Venta.fecha <= endDate)
                    .GroupBy(vp => new {
                        vp.Producto.idProducto,
                        vp.Producto.nombreProducto,
                        vp.Producto.codigoProducto
                    })
                    .Select(g => new TopProduct {
                        ProductId = g.Key.idProducto,
                        ProductCode = g.Key.codigoProducto,
                        ProductName = g.Key.nombreProducto,
                        UnitsSold = g.Sum(x => x.cantidadProducto),
                        TotalSales = g.Sum(x => x.cantidadProducto * x.precioVentaHistorico),
                        Profit = g.Sum(x => (x.precioVentaHistorico - x.precioCompraHistorico) * x.cantidadProducto)
                    })
                    .OrderByDescending(p => p.TotalSales)
                    .Take(count)
                    .ToList();
            }
        }

        public DateTime GetEarliestCategoryDate(string category) {
            using (var context = new MilYUnaNochesEntities()) {
                return context.VentaProducto
                    .Where(vp => vp.Producto.categoria == category)
                    .Min(vp => vp.Venta.fecha);
            }
        }
        #endregion

        #region Métodos para Inventario
        public List<InventoryItem> GetInventoryData() {
            using (var context = new MilYUnaNochesEntities()) {
                return context.Producto
                    .Include(p => p.CompraProducto.Select(cp => cp.CompraProveedor.Proveedor))
                    .Include(p => p.VentaProducto.Select(vp => vp.Venta))
                    .Select(p => new InventoryItem {
                        ProductId = p.idProducto,
                        ProductCode = p.codigoProducto,
                        ProductName = p.nombreProducto,
                        Category = p.categoria,
                        CurrentStock = p.cantidadStock,
                        PurchasePrice = p.precioCompra,
                        SalePrice = p.precioVenta,
                        LastRestockDate = p.CompraProducto
                            .OrderByDescending(cp => cp.CompraProveedor.fecha)
                            .Select(cp => (DateTime?)cp.CompraProveedor.fecha)
                            .FirstOrDefault(),
                        LastSaleDate = p.VentaProducto
                            .OrderByDescending(vp => vp.Venta.fecha)
                            .Select(vp => (DateTime?)vp.Venta.fecha)
                            .FirstOrDefault(),
                        SupplierName = p.CompraProducto
                            .OrderByDescending(cp => cp.CompraProveedor.fecha)
                            .Select(cp => cp.CompraProveedor.Proveedor.nombreProveedor)
                            .FirstOrDefault()
                    })
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.ProductName)
                    .ToList();
            }
        }
        #endregion

        #region Métodos para Ganancias
        public ProfitReportData GetProfitData(DateTime startDate, DateTime endDate) {
            using (var context = new MilYUnaNochesEntities()) {
                var salesData = context.VentaProducto
                    .Include(vp => vp.Venta)
                    .Where(vp => vp.Venta.fecha >= startDate && vp.Venta.fecha <= endDate)
                    .ToList();

                var totalSales = salesData.Sum(vp => vp.cantidadProducto * vp.precioVentaHistorico);
                var totalCosts = salesData.Sum(vp => vp.cantidadProducto * vp.precioCompraHistorico);
                var grossProfit = totalSales - totalCosts;
                var profitMargin = totalSales > 0 ? (grossProfit / totalSales) * 100 : 0;
                var operatingExpenses = GetOperatingExpenses(startDate, endDate);
                var netProfit = grossProfit - operatingExpenses;

                return new ProfitReportData {
                    ReportId = Guid.NewGuid().ToString(),
                    Title = $"Reporte de ganancias {startDate:yyyy-MM-dd} a {endDate:yyyy-MM-dd}",
                    StartDate = startDate,
                    EndDate = endDate,
                    Financials = new FinancialSummary {
                        TotalSales = totalSales,
                        TotalCosts = totalCosts,
                        GrossProfit = grossProfit,
                        ProfitMargin = (decimal)profitMargin,
                        OperatingExpenses = operatingExpenses,
                        NetProfit = netProfit
                    },
                    // Estos puedes poblarlos con datos adicionales si los necesitas
                    CategoryBreakdown = new List<CategoryProfit>(),
                    MonthlyTrend = new List<MonthlyProfit>()
                };
            }
        }

        public List<CategoryProfit> GetCategoryProfitBreakdown(DateTime startDate, DateTime endDate) {
            using (var context = new MilYUnaNochesEntities()) {
                // Primero obtenemos todos los datos necesarios en una sola consulta
                var query = context.VentaProducto
                    .Include(vp => vp.Producto)
                    .Include(vp => vp.Venta)
                    .Where(vp => vp.Venta.fecha >= startDate && vp.Venta.fecha <= endDate)
                    .Select(vp => new {
                        vp.Producto.categoria,
                        vp.cantidadProducto,
                        vp.precioVentaHistorico,
                        vp.precioCompraHistorico
                    })
                    .ToList();

                // Calculamos los totales una sola vez
                var totalSales = query.Sum(vp => vp.cantidadProducto * vp.precioVentaHistorico);
                var totalProfit = query.Sum(vp => (vp.precioVentaHistorico - vp.precioCompraHistorico) * vp.cantidadProducto);

                // Agrupamos y calculamos los porcentajes
                return query
                    .GroupBy(vp => vp.categoria)
                    .Select(g => new CategoryProfit {
                        CategoryName = g.Key ?? "Sin categoría",
                        SalesPercentage = totalSales > 0 ?
                            (decimal)(g.Sum(x => x.cantidadProducto * x.precioVentaHistorico) / totalSales * 100) : 0,
                        ProfitPercentage = totalProfit > 0 ?
                            (decimal)(g.Sum(x => (x.precioVentaHistorico - x.precioCompraHistorico) * x.cantidadProducto) / totalProfit * 100 ): 0
                    })
                    .ToList();
            }
        }

        public List<MonthlyProfit> GetMonthlyProfitTrend(DateTime startDate, DateTime endDate) {
            using (var context = new MilYUnaNochesEntities()) {
                return context.VentaProducto
                    .Include(vp => vp.Venta)
                    .Where(vp => vp.Venta.fecha >= startDate && vp.Venta.fecha <= endDate)
                    .AsEnumerable()
                    .GroupBy(vp => new {
                        vp.Venta.fecha.Year,
                        vp.Venta.fecha.Month
                    })
                    .Select(g => new MonthlyProfit {
                        MonthYear = $"{g.Key.Month:00}/{g.Key.Year}",
                        Sales = g.Sum(x => x.cantidadProducto * x.precioVentaHistorico),
                        Profit = g.Sum(x => (x.precioVentaHistorico - x.precioCompraHistorico) * x.cantidadProducto)
                    })
                    .OrderBy(m => m.MonthYear)
                    .ToList();
            }
        }

        private decimal GetOperatingExpenses(DateTime startDate, DateTime endDate) {
            // Implementar lógica para obtener gastos operativos
            // Esto podría venir de otra tabla en la base de datos
            return 0; // Valor temporal
        }
        #endregion

        #region Métodos auxiliares
        public DateTime GetEarliestSystemDate() {
            using (var context = new MilYUnaNochesEntities()) {
                return context.Venta.Min(v => v.fecha);
            }
        }

        public List<DateRange> GetDateRanges(string periodType, int count, DateTime? limitDate = null) {
            var ranges = new List<DateRange>();
            DateTime endDate = DateTime.Today;
            DateTime systemLimit = limitDate ?? GetEarliestSystemDate();

            for (int i = 0; i < count; i++) {
                DateTime startDate;
                string title;

                try {
                    switch (periodType.ToLower()) {
                        case "diario":
                            startDate = endDate.AddDays(-1);
                            title = endDate.ToString("dd/MM/yyyy");
                            break;
                        case "semanal":
                            startDate = endDate.AddDays(-7);
                            title = $"Semana {GetWeekOfYear(endDate)} {endDate.Year}";
                            break;
                        case "mensual":
                            startDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(-1);
                            endDate = new DateTime(endDate.Year, endDate.Month, 1).AddDays(-1);
                            title = endDate.ToString("MMMM yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("es"));
                            break;
                        case "anual":
                            startDate = new DateTime(endDate.Year - 1, 1, 1);
                            endDate = new DateTime(endDate.Year - 1, 12, 31);
                            title = endDate.ToString("yyyy");
                            break;
                        default:
                            throw new ArgumentException("Tipo de período no válido");
                    }

                    if (startDate < systemLimit) {
                        startDate = systemLimit;
                        title += " (Límite)";
                    }

                    if (startDate >= endDate) break;

                    ranges.Insert(0, new DateRange {
                        StartDate = startDate,
                        EndDate = endDate,
                        Title = title
                    });

                    endDate = startDate.AddDays(-1);

                    if (endDate <= systemLimit) break;
                } catch (ArgumentOutOfRangeException) {
                    break;
                }
            }

            return ranges;
        }

        private int GetWeekOfYear(DateTime date) {
            var ci = System.Globalization.CultureInfo.CurrentCulture;
            return ci.Calendar.GetWeekOfYear(date,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }
        #endregion
    }

    public class DateRange {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
    }
}