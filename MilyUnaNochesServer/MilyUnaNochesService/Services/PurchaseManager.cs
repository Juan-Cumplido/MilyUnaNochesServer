using DataBaseManager.Logic;
using DataBaseManager.Operations;
using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Services {
    public partial class MilyUnaNochesService : IPurchaseManager {
        public int SavePurchase(RegisterPurchase_sv purchase) {
            var purchaseDTO = new RegisterPurchaseDTO {
                IdProveedor = purchase.IdProveedor,
                Fecha = purchase.Fecha,
                Hora = purchase.Hora,
                MontoTotal = purchase.MontoTotal,
                PayMethod = purchase.PayMethod,
                Products = purchase.Products.Select(p => new ProductPurchaseDTO {
                    IdProducto = p.IdProducto,
                    Cantidad = p.Cantidad,
                    MontoProducto = p.MontoProducto
                }).ToList()
            };

            int result = PurchaseOperations.SavePurchase(purchaseDTO);
            return result;

        }
    }
}
