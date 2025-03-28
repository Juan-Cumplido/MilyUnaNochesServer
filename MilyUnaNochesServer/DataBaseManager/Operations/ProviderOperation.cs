using DataBaseManager;
using log4net.Repository.Hierarchy;
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
using System.Web.SessionState;

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

                logger.LogError($"EntityException: An error occurred while retrieving archived suppliers. Exception: {entityException.Message}", entityException);
                providers.Add(operationFailed);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: An error occurred while retrieving archived supplier. Exception: {sqlException.Message}", sqlException);
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
                logger.LogError($"EntityException: An error occurred trying to archive the supplier. Exception: {entityException.Message}", entityException);
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: An error occurred trying to archive the supplier. Exception: {sqlException.Message}", sqlException);
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
                logger.LogError($"EntityException trying to verify if the supplier is registered: {entityException.Message}", entityException);
            } catch (SqlException sqlException) {
                logger.LogError($"SQLException trying to verify if the supplier is registered: {sqlException.Message}", sqlException);
            }
            return verificationResult;
        }
        public static Proveedor GetSupplierInfo(int supplierId) {
            LoggerManager logger = new LoggerManager(typeof(ProviderOperation));
            Proveedor supplierInfo = new Proveedor() {
                idProveedor = Constants.ErrorOperation
            };
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    var query = db.Proveedor.FirstOrDefault(p => p.idProveedor == supplierId);
                    if (query != null) {
                        supplierInfo = query;
                    } else {
                        supplierInfo.idProveedor = Constants.NoDataMatches;
                    }
                }
            } catch (EntityException entityException) {
                logger.LogError($"Error trying to get the suppler information: {entityException.Message}", entityException);
            } catch (Exception exception) {
                logger.LogError($"Error trying to get the suppler information: {exception.Message}", exception);
            }
            return supplierInfo;
        }

        public static int EditSupplierInfo(Proveedor newProviderInfo, Direccion newAdressInfo) {
            LoggerManager logger = new LoggerManager(typeof(ProviderOperation));
            int operationResult = Constants.ErrorOperation;

            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    using (var transaction = db.Database.BeginTransaction()) {
                        Proveedor existingProvider = db.Proveedor.FirstOrDefault(p => p.idProveedor == newProviderInfo.idProveedor);

                        if (existingProvider == null) {
                            transaction.Rollback();
                            operationResult = Constants.NoDataMatches;
                            logger.LogError("Provider not found");
                            return operationResult;
                        }

                        Direccion existingAddress = db.Direccion.Find(existingProvider.idDireccion);
                        if (existingAddress == null) {
                            transaction.Rollback();
                            operationResult = Constants.NoDataMatches;
                            logger.LogError("Address not found");
                            return operationResult;
                        }

                        existingProvider.correo = newProviderInfo.correo;
                        existingProvider.nombreProveedor = newProviderInfo.nombreProveedor;
                        existingProvider.contacto = newProviderInfo.contacto;
                        existingProvider.telefono = newProviderInfo.telefono;

                        int addressUpdateResult = AddressOperation.EditAddress(db, existingAddress, newAdressInfo);

                        if (addressUpdateResult == Constants.SuccessOperation) {
                            db.SaveChanges();
                            transaction.Commit();
                            operationResult = Constants.SuccessOperation;
                        } else {
                            transaction.Rollback();
                            logger.LogError("Error updating address.");
                            operationResult = Constants.ErrorOperation;
                        }
                    }
                }
            } catch (EntityException entityException) {
                logger.LogError($"Error trying to update the suppler information: {entityException.Message}", entityException);
            } catch (Exception exception) {
                logger.LogError($"Error trying to update the suppler information: {exception.Message}", exception);
            }
            return operationResult;
        }
    }
}
