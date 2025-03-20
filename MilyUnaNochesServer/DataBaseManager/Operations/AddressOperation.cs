using log4net.Repository.Hierarchy;
using MilyUnaNochesService.Utilities;
using System;
using log4net;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DataBaseManager.Operations {
    public static class AddressOperation {
        public static int CreateAddress(Direccion direccion) {
            LoggerManager logger = new LoggerManager(typeof(AddressOperation));
            int idCreated = Constants.ErrorOperation;
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    db.Direccion.Add(direccion);
                    db.SaveChanges();
                    idCreated = direccion.idDireccion;
                }
            } catch (EntityException entityException) {
                logger.LogError($"DbEntityValidationException: Error trying to register address. Exception: {entityException.Message}", entityException);
            } catch (Exception exception) {
                logger.LogError($"Exception: Error trying to register address. Exception: {exception.Message}", exception);
            }
            return idCreated;
        }
        public static Direccion GetAddress(int idDireccion) {
            Direccion directionResult = new Direccion();
            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    var directionEntity = db.Direccion.FirstOrDefault(d => d.idDireccion == idDireccion);

                    if (directionEntity == null) {
                        directionResult.idDireccion = Constants.NoDataMatches;
                    } else {
                        directionResult = directionEntity;
                    }
                }
            } catch (EntityException entityException) {
                directionResult.idDireccion = Constants.ErrorOperation;
            } catch (Exception exception) {
                directionResult.idDireccion = Constants.ErrorOperation;
            }
            return directionResult;
        }

        public static bool DeleteAddress(int idDireccion) {
            LoggerManager logger = new LoggerManager(typeof(AddressOperation));
            bool isDeleted = false;

            try {
                using (MilYUnaNochesEntities db = new MilYUnaNochesEntities()) {
                    var direccionToDelete = db.Direccion.FirstOrDefault(d => d.idDireccion == idDireccion);

                    if (direccionToDelete != null) {
                        db.Direccion.Remove(direccionToDelete);
                        db.SaveChanges();
                        isDeleted = true;
                    }
                }
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error al intentar eliminar la dirección. Excepción: {entityException.Message}", entityException);
            } catch (DbEntityValidationException dbEntityValidationException) {
                logger.LogError($"DbEntityValidationException: Error de validación al intentar eliminar la dirección. Excepción: {dbEntityValidationException.Message}", dbEntityValidationException);
            } catch (Exception exception) {
                logger.LogError($"Exception: Error inesperado al intentar eliminar la dirección. Excepción: {exception.Message}", exception);
            }
            return isDeleted;
        }
    }
}