using DataBaseManager.Logic;
using DataBaseManager.Operations;
using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Logic;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Services {
    public partial class MilyUnaNochesService : IPurchaseManager {
        public List<ConsultPurchase_SV> GetPurchases() {
            List<ConsultPurchase_SV> purchasesSv = new List<ConsultPurchase_SV>();

            // Se obtiene la lista de compras registrada en la capa de acceso a datos
            List<ConsultPurchaseDTO> purchasesDTO = PurchaseOperations.GetRegisteredPurchases();

            // Se mapea cada objeto ConsultPurchaseDTO al contrato de servicio ConsultPurchase_SV
            foreach (var pDTO in purchasesDTO) {
                ConsultPurchase_SV purchaseSv = new ConsultPurchase_SV() {
                    providerName = pDTO.providerName,
                    providerContact = pDTO.providerContact,
                    Fecha = pDTO.Fecha,
                    Hora = pDTO.Hora,
                    purchasedProducts = pDTO.purchasedProducts,
                    payMethod = pDTO.payMethod,
                    amountPaid = pDTO.amountPaid
                };
                purchasesSv.Add(purchaseSv);
            }
            return purchasesSv;
        }

        public int SavePurchase(RegisterPurchase_sv purchase) {
            LoggerManager logger = new LoggerManager(typeof(AddressOperation));
            var purchaseDTO = new RegisterPurchaseDTO {
                IdProveedor = purchase.IdProveedor,
                ContactoProveedor = purchase.ContactoProveedor,
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

            try {
                int result = PurchaseOperations.SavePurchase(purchaseDTO);
                return result;
            } catch (KeyNotFoundException keyNotFound) {
                logger.LogError("Product ID doesnt exist in table Products");
                return Constants.ErrorOperation;
            }
        }
    }
}
