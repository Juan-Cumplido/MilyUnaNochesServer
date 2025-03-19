using MilyUnaNochesService.Utilities;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web.ModelBinding;
using System;


namespace DataBaseManager.Operations {
    internal class UserOperation {
namespace DataBaseManager.Operations
{
    public class UserOperation
    {
        public int addClient(Usuario user)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            int result = Constants.ErrorOperation;
            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    using (var dataBaseContextTransaction = dataBaseContext.Database.BeginTransaction())
                    {
                        try
                        {
                            
                            var newUser = new Usuario
                            {
                                nombre = user.nombre,
                                primerApellido = user.primerApellido,
                                segundoApellido = user.segundoApellido,
                                correo = user.correo,
                                telefono = user.telefono,
                                estadoUsuario = user.estadoUsuario,
                            };
                            dataBaseContext.Usuario.Add(newUser);
                            dataBaseContext.SaveChanges();
                            int lastIdUserInserted = newUser.idUsuario;

                            var newClient = new Cliente
                            {
                                idUsuario = lastIdUserInserted,
                            };
                            dataBaseContext.Cliente.Add(newClient);
                            dataBaseContext.SaveChanges();
                            
                           
                            dataBaseContextTransaction.Commit();
                            result = Constants.SuccessOperation;
                        }
                        catch (DbUpdateException updateException)
                        {
                            logger.LogWarn(updateException);
                            dataBaseContextTransaction.Rollback();
                            result = Constants.ErrorOperation;
                        }
                        catch (SqlException sqlException)
                        {
                            logger.LogError(sqlException);
                            dataBaseContextTransaction.Rollback();
                            result = Constants.ErrorOperation;
                        }
                    }
                }
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
            }
            return result;
        }

        public int VerifyExistingClientIntoDataBase(string name, string firstLastName, string secondLastName)
        {
            int verificationResult = -1;
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var existingClient = dataBaseContext.Usuario.FirstOrDefault(client => client.nombre == name && client.primerApellido == firstLastName && client.segundoApellido == secondLastName);
                    if (existingClient != null)
                    {
                        verificationResult = Constants.DataMatches;
                    }
                    else
                    {
                        verificationResult = Constants.NoDataMatches;
                    }
                }
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                verificationResult = Constants.ErrorOperation;
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
                verificationResult = Constants.ErrorOperation;
            }
            return verificationResult;
        }
    }
}
