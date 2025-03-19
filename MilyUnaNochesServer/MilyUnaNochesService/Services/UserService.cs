using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.UtilitiesService;
using DataBaseManager.Operations;
using DataBaseManager;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MilyUnaNochesService.Services
{
    public partial class MilyUnaNochesService : IUserManager
    {
        public int AddClient(Contracts.Usuario user)
        {
            UserOperation operations = new UserOperation();
            DataBaseManager.Usuario newUser = new DataBaseManager.Usuario()
            {
                nombre = user.nombre,
                primerApellido = user.primerApellido,
                segundoApellido = user.segundoApellido,
                correo = user.correo,
                telefono = user.telefono,
                estadoUsuario = "Active",
            };
            int insertionResult = operations.addClient(newUser);
            return insertionResult;
        }

        public int AddEmployee(Contracts.Usuario user, Contracts.Direccion address, Contracts.Empleado employee, Contracts.Acceso acces)
        {
            UserOperation operations = new UserOperation();
            DataBaseManager.Usuario newUser = new DataBaseManager.Usuario()
            {
                nombre = user.nombre,
                primerApellido = user.primerApellido,
                segundoApellido = user.segundoApellido,
                correo = user.correo,
                telefono = user.telefono,
                estadoUsuario = "Active",
            };

            DataBaseManager.Direccion newAddress = new DataBaseManager.Direccion()
            {
                calle = address.calle,
                numero = address.numero,
                codigoPostal = address.codigoPostal,
                ciudad = address.ciudad,
            };
            DataBaseManager.Empleado newEmployee = new DataBaseManager.Empleado()
            {
                tipoEmpleado = employee.tipoEmpleado,
            };
            DataBaseManager.Acceso newAccess = new DataBaseManager.Acceso()
            {
                usuario = acces.usuario,
                contraseña = acces.contraseña,
            };

            int insertionResult = operations.addEmployee(newUser, newAddress, newEmployee, newAccess);
            return insertionResult;
        }

        public int ArchiveClient(int idUsuario)
        {
            UserOperation operations = new UserOperation();
            int verificationResult = operations.ArchiveClient(idUsuario);
            return verificationResult;
        }

        public int ArchiveEmployee(int idUsuario)
        {
            UserOperation operations = new UserOperation();
            int verificationResult = operations.ArchiveEmployee(idUsuario);
            return verificationResult;
        }

        public List<Contracts.Usuario> GetUserProfileByNamePhone(string searchTerm)
        {
            UserOperation userOperation = new UserOperation();
            var usersObtained = userOperation.GetClientsByNameOrPhone(searchTerm);

            List<Contracts.Usuario> profilesObtained = new List<Contracts.Usuario>();

            if (usersObtained != null && usersObtained.Any())
            {
                profilesObtained = usersObtained.Select(user => new Contracts.Usuario()
                {
                    idUsuario = user.idUsuario,
                    nombre = user.nombre,
                    primerApellido = user.primerApellido,
                    segundoApellido = user.segundoApellido,
                    correo = user.correo,
                    telefono = user.telefono
                }).ToList();
            }

            return profilesObtained;
        }

        public List<Contracts.Empleado> GetActiveEmployeesBySearchTerm(string searchTerm)
        {
            UserOperation userOperation = new UserOperation();
            var employeesObtained = userOperation.GetActiveEmployeesBySearchTerm(searchTerm);

            List<Contracts.Empleado> profilesObtained = new List<Contracts.Empleado>();

            if (employeesObtained != null && employeesObtained.Any())
            {
                profilesObtained = employeesObtained.Select(employee => new Contracts.Empleado()
                {
                    idUsuario = employee.idUsuario,
                    nombre = employee.nombre,
                    primerApellido = employee.primerApellido,
                    segundoApellido = employee.segundoApellido,
                    correo = employee.correo,
                    telefono = employee.telefono,
                    tipoEmpleado = employee.tipoEmpleado,
                        calle = employee.calle,
                        numero = employee.numero,
                        codigoPostal = employee.codigoPostal,
                        ciudad = employee.ciudad
                    
                }).ToList();
            }

            

            return profilesObtained;
        }



        public bool VerifyAccess(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public int VerifyExistinClient(string name, string firstLastName, string secondLastName)
        {
            UserOperation operations = new UserOperation();
            int verificationResult = operations.VerifyExistingClientIntoDataBase(name, firstLastName, secondLastName);
            return verificationResult;
        }

        public int VerifyExistinEmployee(string email, string phone)
        {
            
            UserOperation operations = new UserOperation();
            int verificationResult = operations.VerifyExistingEmployeeIntoDataBase(email, phone );
            return verificationResult;
        }

        

    }
}
