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

        public int VerifyCredentialsFromDataBase(string username)
        {
            int verificationResult = -1;
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var existingAccount = dataBaseContext.Acceso.FirstOrDefault(access => access.usuario == username);
                    if (existingAccount != null)
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

        public int VerifyPasswordCredentialsFromDataBase(string username, string password)
        {
            int verificationResult = -1;
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var existingAccount = dataBaseContext.Acceso.Where(access => access.usuario == username).FirstOrDefault();
                    if (existingAccount != null && existingAccount.contraseña == password)
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
        public string searchEmployeeType(string username, string password)
        {
            string employeeType = "No encontrado";
            LoggerManager logger = new LoggerManager(this.GetType());
            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                     var existingAccount = dataBaseContext.Acceso
                        .Where(access => access.usuario == username && access.contraseña == password)
                        .Select(access => access.idEmpleado)
                        .FirstOrDefault();

                    if (existingAccount != null)
                    {
                        var employee = dataBaseContext.Empleado
                            .Where(emp => emp.idEmpleado == existingAccount)
                            .Select(emp => emp.tipoEmpleado)
                            .FirstOrDefault();

                        if (!string.IsNullOrEmpty(employee))
                        {
                            employeeType = employee;
                        }
                    }
                }
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                employeeType = "Error en la operación";
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
                employeeType = "Error en la operación";
            }
            return employeeType;
        }

        public EmployeeData GetUserDataFromDataBase(string username, string password)
        {
            EmployeeData dataObtained = null;
            LoggerManager logger = new LoggerManager(this.GetType());

            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var userData = (from acceso in dataBaseContext.Acceso
                                    join empleado in dataBaseContext.Empleado on acceso.idEmpleado equals empleado.idEmpleado
                                    join usuario in dataBaseContext.Usuario on empleado.idUsuario equals usuario.idUsuario
                                    where acceso.usuario == username && acceso.contraseña == password
                                    select new EmployeeData
                                    {
                                        idAcceso = acceso.idAcceso,
                                        idEmpleado = empleado.idEmpleado,
                                        tipoEmpleado = empleado.tipoEmpleado,
                                        nombre = usuario.nombre,
                                        primerApellido = usuario.primerApellido,
                                        segundoApellido = usuario.segundoApellido,
                                        correo = usuario.correo,
                                        telefono = usuario.telefono
                                    }).FirstOrDefault();

                    dataObtained = userData ?? new EmployeeData { idEmpleado = Constants.NoDataMatches };
                }
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                dataObtained = new EmployeeData { idEmpleado = Constants.ErrorOperation };
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
                dataObtained = new EmployeeData { idEmpleado = Constants.ErrorOperation };
            }

            return dataObtained;
        }


        public EmployeeData GetEmployeeByIdUsuario(int idUsuario)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            EmployeeData employee = null;

            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var result = (from e in dataBaseContext.Empleado
                                  join u in dataBaseContext.Usuario on e.idUsuario equals u.idUsuario
                                  join d in dataBaseContext.Direccion on e.idDireccion equals d.idDireccion
                                  where u.idUsuario == idUsuario
                                  select new EmployeeData
                                  {
                                      idEmpleado = e.idEmpleado,
                                      idUsuario = u.idUsuario,
                                      nombre = u.nombre,
                                      primerApellido = u.primerApellido,
                                      segundoApellido = u.segundoApellido,
                                      telefono = u.telefono,
                                      correo = u.correo,
                                      calle = d.calle,
                                      codigoPostal = d.codigoPostal,
                                      numero = d.numero,
                                      ciudad = d.ciudad,
                                      tipoEmpleado = e.tipoEmpleado
                                  }).FirstOrDefault();

                    employee = result ?? new EmployeeData { idUsuario = Constants.NoDataMatches };
                }
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                employee = new EmployeeData { idUsuario = Constants.ErrorOperation };
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
                employee = new EmployeeData { idUsuario = Constants.ErrorOperation };
            }

            return employee;
        }

        public int UpdateEmployeeByIdUsuario(EmployeeData updatedData)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            int result = Constants.ErrorOperation;

            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var usuario = dataBaseContext.Usuario.FirstOrDefault(u => u.idUsuario == updatedData.idUsuario);
                    var empleado = dataBaseContext.Empleado.FirstOrDefault(e => e.idUsuario == updatedData.idUsuario);

                    if (usuario != null && empleado != null)
                    {
                        usuario.nombre = updatedData.nombre;
                        usuario.primerApellido = updatedData.primerApellido;
                        usuario.segundoApellido = updatedData.segundoApellido;
                        usuario.telefono = updatedData.telefono;
                        usuario.correo = updatedData.correo;

                        var direccion = dataBaseContext.Direccion.FirstOrDefault(d => d.idDireccion == empleado.idDireccion);
                        if (direccion != null)
                        {
                            direccion.calle = updatedData.calle;
                            direccion.codigoPostal = updatedData.codigoPostal;
                            direccion.numero = updatedData.numero;
                            direccion.ciudad = updatedData.ciudad;
                        }

                        empleado.tipoEmpleado = updatedData.tipoEmpleado;

                        dataBaseContext.SaveChanges();
                        result = Constants.SuccessOperation;
                    }
                    else
                    {
                        result = Constants.NoDataMatches;
                    }
                }
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
            }

            return result;
        }

        public ClientData GetClientByIdUsuario(int idUsuario)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            ClientData client = null;

            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var result = (from u in dataBaseContext.Usuario
                                  where u.idUsuario == idUsuario
                                  select new ClientData
                                  {
                                      
                                      idUsuario = u.idUsuario,
                                      nombre = u.nombre,
                                      primerApellido = u.primerApellido,
                                      segundoApellido = u.segundoApellido,
                                      telefono = u.telefono,
                                      correo = u.correo,
                                     
                                  }).FirstOrDefault();

                    client = result ?? new ClientData { idUsuario = Constants.NoDataMatches };
                }
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
                client = new ClientData { idUsuario = Constants.ErrorOperation };
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
                client = new ClientData { idUsuario = Constants.ErrorOperation };
            }

            return client;
        }

        public int UpdateClientByIdUsuario(ClientData updatedData)
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            int result = Constants.ErrorOperation;

            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var usuario = dataBaseContext.Usuario.FirstOrDefault(u => u.idUsuario == updatedData.idUsuario);

                    if (usuario != null)
                    {
                        usuario.nombre = updatedData.nombre;
                        usuario.primerApellido = updatedData.primerApellido;
                        usuario.segundoApellido = updatedData.segundoApellido;
                        usuario.telefono = updatedData.telefono;
                        usuario.correo = updatedData.correo;
                        dataBaseContext.SaveChanges();
                        result = Constants.SuccessOperation;
                    }
                    else
                    {
                        result = Constants.NoDataMatches;
                    }
                }
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
            }
            catch (EntityException entityException)
            {
                logger.LogFatal(entityException);
            }

            return result;
        }

        public int GetClientIdByUserId(int idUsuario)
        {
            LoggerManager logger = new LoggerManager(this.GetType());

            try
            {
                using (var dataBaseContext = new MilYUnaNochesEntities())
                {
                    var cliente = dataBaseContext.Cliente
                        .FirstOrDefault(c => c.idUsuario == idUsuario);

                    return cliente.idCliente;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                return -1;
            }
        }
    }
}