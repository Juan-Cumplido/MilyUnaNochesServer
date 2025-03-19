using System;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace MilyUnaNochesService.Contracts
{
    [ServiceContract]
    public interface IUserManager
    {
        [OperationContract]
        int AddClient(Usuario usuario);


        [OperationContract]
        bool VerifyAccess(string username, string password);

        [OperationContract]
        int VerifyExistinClient(string name,string firstLastName, string secondLastName);
    }

    [DataContract]
    public class Usuario
    {
        [DataMember]
        public int idUsuario { get; set; }
        [DataMember]
        public string nombre { get; set; }
        [DataMember]
        public string primerApellido { get; set; }
        [DataMember]
        public string segundoApellido { get; set; }
        [DataMember]
        public string correo { get; set; }
        [DataMember]
        public string telefono { get; set; }
        [DataMember]
        public string estadoUsuario { get; set; }

    }

    [DataContract]
    public class Acceso
    {
        [DataMember]
        public int idAcceso { get; set; }
        [DataMember]
        public string idEmpleado { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string password { get; set; }
       
    }

    [DataContract]
    public class UserDireccion
    {
        [DataMember]
        public int idDireccion { get; set; }
        [DataMember]
        public string calle { get; set; }
        [DataMember]
        public string numero { get; set; }
        [DataMember]
        public string codigoPostal { get; set; }
        [DataMember]
        public string ciudad { get; set; }
    }
}
