using System;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace MilyUnaNochesService.Contracts
{
    [ServiceContract]
    public interface IUserManager
    {
        /// <summary>
        /// Obtains a user profile by his username
        /// </summary>
        /// <param name="profile">Contains the data of the profile to add</param>
        /// <returns>Returns 1 if the insertion was succesfull</returns>
        [OperationContract]
        int AddUser(Profile profile);
    }

    [DataContract]
    public class Profile : AccessAccount
    { 
    }

    [DataContract]
    public class AccessAccount
    {
        [DataMember]
        public int idAcceso { get; set; }
    }

}
