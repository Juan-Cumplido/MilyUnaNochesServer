using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
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
                        idDireccion = provider.idDireccion,
                        estadoProveedor = "ACTIVO"
                    };

                    db.Proveedor.Add(newProvider);
                    db.SaveChanges();
                    operationStatus = Constants.SuccessOperation;
                }
            } catch (DbEntityValidationException dbEntityValidationException) {
                logger.LogError($"DbEntityValidationException: Error trying to register user: {provider.nombreProveedor}. Exception: {dbEntityValidationException.Message}", dbEntityValidationException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while registering user {provider.nombreProveedor}. Exception: {generalException.Message}", generalException);
            }
            return operationStatus;
        }

        public static List<Proveedor> GetRegisteredProviders() {
            LoggerManager logger = new LoggerManager(typeof(ProviderOperation));
            Proveedor operationFailed = new Proveedor() {
                idProveedor = Constants.ErrorOperation
            };
            List<Proveedor> providers = new List<Proveedor>();
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    providers = db.Proveedor.ToList();
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: An error occurred while retrieving providers. Exception: {entityException.Message}", entityException);
                providers.Add(operationFailed);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: An error occurred while retrieving providers. Exception: {sqlException.Message}", sqlException);
                providers.Add(operationFailed);
            }
            return providers;
        }

        public static int archiveProvider(int idProvider) {
            LoggerManager logger = new LoggerManager(typeof(ProviderOperation));
            int operationStatus = Constants.ErrorOperation;
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    Proveedor provider = db.Proveedor.FirstOrDefault(p => p.idProveedor == idProvider);
                    if (provider != null) {
                        provider.estadoProveedor = "Inactivo";
                        db.SaveChanges();
                        operationStatus = Constants.SuccessOperation;
                    } else {
                        operationStatus = Constants.NoDataMatches;
                    }
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: An error occurred trying to archive the provider. Exception: {entityException.Message}", entityException);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: An error occurred trying to archive the provider. Exception: {sqlException.Message}", sqlException);
            }
            return operationStatus;
        }

        public static int DeleteProvider(int idProvider) {
            LoggerManager logger = new LoggerManager(typeof(ProviderOperation));
            int operationStatus = Constants.ErrorOperation;
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    using (var transaction = db.Database.BeginTransaction()) {
                        try {
                            Proveedor provider = db.Proveedor.FirstOrDefault(p => p.idProveedor == idProvider);
                            if (provider == null) {
                                return Constants.NoDataMatches;
                            }
                            int idDireccion = provider.idDireccion;

                            db.Proveedor.Remove(provider);
                            db.SaveChanges();

                            Direccion direccion = db.Direccion.Find(idDireccion);
                            if (direccion != null) {
                                db.Direccion.Remove(direccion);
                                db.SaveChanges();
                            }

                            transaction.Commit();
                            operationStatus = Constants.SuccessOperation;
                        } catch (Exception ex) {
                            transaction.Rollback();
                            logger.LogError($"Error al eliminar proveedor y dirección: {ex.Message}", ex);
                            operationStatus = Constants.ErrorOperation;
                        }
                    }
                }
            } catch (Exception ex) {
                logger.LogError($"Error al acceder a la base de datos: {ex.Message}", ex);
                operationStatus = Constants.ErrorOperation;
            }
            return operationStatus;
        }


    }
}