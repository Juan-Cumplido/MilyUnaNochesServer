using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.UtilitiesService;
using DataBaseManager.Operations;

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
    }
}
