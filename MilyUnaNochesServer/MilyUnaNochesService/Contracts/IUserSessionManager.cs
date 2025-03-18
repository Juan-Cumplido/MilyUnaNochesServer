using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Contracts
{
    [ServiceContract]
    public interface IUserSessionManager
    {
        [OperationContract]
        int Connect(UserSession user);
    }

    [DataContract]
    public class UserSession
    {
        [DataMember]
        public string usuario { get; set; }
        [DataMember]
        public int idAcceso { get; set; }
        public override bool Equals(object obj)
        {
            bool comparation = false;
            if (obj is UserSession other)
            {
                comparation = usuario.Equals(other.usuario) &&
                    idAcceso.Equals(other.idAcceso);
            }
            return comparation;
        }

        public override int GetHashCode()
        {
            int hashUsername = usuario?.GetHashCode() ?? 0;
            int hashIdAccount = idAcceso.GetHashCode();
            return hashUsername ^ hashIdAccount;
        }
    }

}
