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
    }
}