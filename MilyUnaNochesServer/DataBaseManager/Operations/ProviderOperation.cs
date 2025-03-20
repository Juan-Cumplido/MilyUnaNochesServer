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
                    providers = db.Proveedor.Where(p => p.estadoProveedor == "ACTIVO").ToList();
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: An error occurred while retrieving active providers. Exception: {entityException.Message}", entityException);
                providers.Add(operationFailed);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: An error occurred while retrieving active providers. Exception: {sqlException.Message}", sqlException);
                providers.Add(operationFailed);
            }
            return providers;
        }

        public static List<Proveedor> GetArchivedProviders() {
            LoggerManager logger = new LoggerManager(typeof(ProviderOperation));
            Proveedor operationFailed = new Proveedor() {
                idProveedor = Constants.ErrorOperation
            };
            List<Proveedor> providers = new List<Proveedor>();
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    providers = db.Proveedor.Where(p => p.estadoProveedor == "ARCHIVADO").ToList();
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: An error occurred while retrieving archived providers. Exception: {entityException.Message}", entityException);
                providers.Add(operationFailed);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: An error occurred while retrieving archived providers. Exception: {sqlException.Message}", sqlException);
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
                        provider.estadoProveedor = "ARCHIVADO";
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

        public static int UnArchiveProvider(int idProvider) {
            LoggerManager logger = new LoggerManager(typeof(ProviderOperation));
            int operationStatus = Constants.ErrorOperation;
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    Proveedor provider = db.Proveedor.FirstOrDefault(p => p.idProveedor == idProvider);
                    if (provider != null) {
                        provider.estadoProveedor = "ACTIVO";
                        db.SaveChanges();
                        operationStatus = Constants.SuccessOperation;
                    } else {
                        operationStatus = Constants.NoDataMatches;
                    }
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: An error occurred trying to unarchive the provider. Exception: {entityException.Message}", entityException);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: An error occurred trying to unarchive the provider. Exception: {sqlException.Message}", sqlException);
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
                            logger.LogError($"Error trying to delete provider and his/her direction: {ex.Message}", ex);
                            operationStatus = Constants.ErrorOperation;
                        }
                    }
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: {entityException.Message}", entityException);
                operationStatus = Constants.ErrorOperation;
            } catch (SqlException sqlException) {
                logger.LogError($"SQLException: {sqlException.Message}", sqlException);
            }
            return operationStatus;
        }

        public static int VerifyProviderInDataBase(string providerName) {
            LoggerManager logger = new LoggerManager(typeof(ProviderOperation));
            int verificationResult = Constants.ErrorOperation;
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    if (db.Proveedor.Any(p => p.nombreProveedor == providerName)) {
                        verificationResult = Constants.DataMatches;
                    } else {
                        verificationResult = Constants.NoDataMatches;
                    }
                }
            } catch (EntityException entityException) {
                logger.LogError($"Error al verificar proveedor: {entityException.Message}", entityException);
            } catch (SqlException sqlException) {
                logger.LogError($"Error de SQL al verificar proveedor: {sqlException.Message}", sqlException);
            }
            return verificationResult;
        }
    }
}
