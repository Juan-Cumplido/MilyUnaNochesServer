using MilyUnaNochesService.Utilities;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web.ModelBinding;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using log4net.Repository.Hierarchy;


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

        public int addEmployee(Usuario user, Direccion address, Empleado employee, Acceso acces)
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

                            var newAddress = new Direccion
                            {
                                calle = address.calle,
                                numero = address.numero,
                                codigoPostal = address.codigoPostal,
                                ciudad = address.ciudad,
                            };
                            dataBaseContext.Direccion.Add(newAddress);
                            dataBaseContext.SaveChanges();
                            int lastIdAddressInserted = newAddress.idDireccion;

                            var newEmployee = new Empleado
                            {
                                idUsuario = lastIdUserInserted,
                                tipoEmpleado = employee.tipoEmpleado,
                                idDireccion = lastIdAddressInserted,

                            };
                            dataBaseContext.Empleado.Add(newEmployee);
                            dataBaseContext.SaveChanges();
                            int lastIdEmployeeInserted = newEmployee.idEmpleado;


                            var newAccess = new Acceso
                            {
                                idEmpleado = lastIdEmployeeInserted,
                                usuario = acces.usuario,
                                contraseña = acces.contraseña,
                            };
                            dataBaseContext.Acceso.Add(newAccess);
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

        public int VerifyExistingEmployeeIntoDataBase(string email, string phone)
        {
            int verificationResult = -1;
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var existingClient = dataBaseContext.Usuario.FirstOrDefault(client => client.correo == email && client.telefono == phone);
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

        public List<ClientData> GetClientsByNameOrPhone(string searchTerm)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            List<ClientData> clientsFound = new List<ClientData>();

            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var clients = (from usuario in dataBaseContext.Usuario
                                   join cliente in dataBaseContext.Cliente on usuario.idUsuario equals cliente.idUsuario
                                   where (usuario.nombre.Contains(searchTerm)
                                       || usuario.primerApellido.Contains(searchTerm)
                                       || usuario.segundoApellido.Contains(searchTerm)
                                       || usuario.telefono.Contains(searchTerm))
                                       && usuario.estadoUsuario == "Active" 
                                   select new ClientData
                                   {
                                       idUsuario = usuario.idUsuario,
                                       nombre = usuario.nombre,
                                       primerApellido = usuario.primerApellido,
                                       segundoApellido = usuario.segundoApellido,
                                       correo = usuario.correo,
                                       telefono = usuario.telefono
                                   }).ToList();

                    if (clients.Any())
                    {
                        clientsFound = clients;
                    }
                    else
                    {
                        clientsFound.Add(new ClientData { idUsuario = Constants.NoDataMatches });
                    }
                }
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                clientsFound.Clear();
                clientsFound.Add(new ClientData { idUsuario = Constants.ErrorOperation });
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
                clientsFound.Clear();
                clientsFound.Add(new ClientData { idUsuario = Constants.ErrorOperation });
            }

            return clientsFound;
        }

        public List<EmployeeData> GetActiveEmployeesBySearchTerm(string searchTerm)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            List<EmployeeData> employees = new List<EmployeeData>();

            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var result = (from e in dataBaseContext.Empleado
                                  join u in dataBaseContext.Usuario on e.idUsuario equals u.idUsuario
                                  join d in dataBaseContext.Direccion on e.idDireccion equals d.idDireccion
                                  where u.estadoUsuario == "Active" && (
                                        u.nombre.Contains(searchTerm) ||
                                        u.primerApellido.Contains(searchTerm) ||
                                        u.segundoApellido.Contains(searchTerm) ||
                                        u.correo.Contains(searchTerm) ||
                                        u.telefono.Contains(searchTerm) ||
                                        e.tipoEmpleado.Contains(searchTerm) ||
                                        d.calle.Contains(searchTerm) ||
                                        d.numero.Contains(searchTerm) ||
                                        d.codigoPostal.Contains(searchTerm) ||
                                        d.ciudad.Contains(searchTerm))

                                  select new EmployeeData
                                  {
                                      idUsuario = u.idUsuario,
                                      nombre = u.nombre,
                                      primerApellido = u.primerApellido,
                                      segundoApellido = u.segundoApellido,
                                      correo = u.correo,
                                      telefono = u.telefono,
                                      tipoEmpleado = e.tipoEmpleado,
                                      calle = d.calle,
                                      numero = d.numero,
                                      codigoPostal = d.codigoPostal,
                                      ciudad = d.ciudad
                                  }).ToList();


                    if (result.Any())
                    {
                        employees = result;
                    }
                    else
                    {
                        employees.Add(new EmployeeData { idUsuario = Constants.NoDataMatches });
                    }
                }
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                employees.Clear();
                employees.Add(new EmployeeData { idUsuario = Constants.ErrorOperation });
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
                employees.Clear();
                employees.Add(new EmployeeData { idUsuario = Constants.ErrorOperation });
            }

            return employees;
        }



        public int ArchiveClient(int  idUsuario)
        {
            int verificationResult = -1;
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var usuario = dataBaseContext.Usuario.Find(idUsuario);
                    if (usuario != null)
                    {
                        usuario.estadoUsuario = "Filed";
                        dataBaseContext.SaveChanges();
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


        public int ArchiveEmployee(int idUsuario)
        {
            int verificationResult = -1;
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var usuario = dataBaseContext.Usuario.Find(idUsuario);
                    if (usuario != null)
                    {
                        usuario.estadoUsuario = "Filed";
                        dataBaseContext.SaveChanges();
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
