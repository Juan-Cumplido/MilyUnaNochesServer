using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Services
{
    public partial class MilyUnaNochesService : IUserSessionManager
    {
        private static readonly List<UserSession> _usersConnected = new List<UserSession>();
        public int Connect(UserSession user)
        {
            int resultConnection = Constants.ErrorOperation;
            if (!_usersConnected.Contains(user))
            {
                _usersConnected.Add(user);
                
                resultConnection = Constants.SuccessOperation;
            }
            return resultConnection;
        }

        public int Disconnect(UserSession user, bool isInMatch)
        {
            int resultDisconnection;
            if (_usersConnected.Exists(userToDisconnect => userToDisconnect.usuario == user.usuario))
            {
                Acceso userProfile = new Acceso()
                {
                    usuario = user.usuario
                };
                _usersConnected.RemoveAll(userToDisconnect => userToDisconnect.usuario == user.usuario);
               

                resultDisconnection = Constants.SuccessOperation;
            }
            else
            {
                resultDisconnection = Constants.NoDataMatches;
            }
            return resultDisconnection;
        }

        public bool VerifyConnectivity(UserSession user)
        {
            bool resultVerification = false;
            if (_usersConnected.Contains(user))
            {
                resultVerification = true;
            }
            return resultVerification;
        }
       
    }
}
