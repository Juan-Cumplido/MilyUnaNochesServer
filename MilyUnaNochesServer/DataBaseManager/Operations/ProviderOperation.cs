using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager.Operations {
    public static class ProviderOperation { 
        public static int AddProvider(Proveedor provider) {
            LoggerManager logger = new LoggerManager(typeof(ProviderOperation));
            int operationStatus = Constants.ErrorOperation;
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    Proveedor newProvider = new Proveedor {
                        nombreProveedor = provider.nombreProveedor,
                        contacto = provider.contacto,
                        telefono = provider.telefono,
                        correo = provider.correo,
                        estadoProveedor = provider.estadoProveedor,
                        idDireccion = provider.idDireccion,
                    };
                }
            } catch (DbEntityValidationException dbEntityValidationException) {
                logger.LogError($"DbEntityValidationException: Error trying to register user: {provider.nombreProveedor}. Exception: {dbEntityValidationException.Message}", dbEntityValidationException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while registering user {provider.nombreProveedor}. Exception: {generalException.Message}", generalException);
            }
            operationStatus = Constants.SuccessOperation;
            return operationStatus;
        }
    }
}
