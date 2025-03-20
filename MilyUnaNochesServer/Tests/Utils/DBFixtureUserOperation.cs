using DataBaseManager;
using DataBaseManager.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Utils {
        public class DBFixtureUserOperation : IDisposable {
            public int GetUserIdByName(string name) {
                int id = -1;
                using (var db = new MilYUnaNochesEntities()) {
                    var user = db.Usuario.FirstOrDefault(u => u.nombre == name);
                    if (user != null)
                        id = user.idUsuario;
                }
                return id;
            }

            public void Dispose() {
                using (var db = new MilYUnaNochesEntities()) {
                    var users = db.Usuario.Where(u => u.nombre.StartsWith("Test")).ToList();
                    foreach (var user in users) {
                        UserOperation userOp = new UserOperation();
                        userOp.ArchiveClient(user.idUsuario);
                        userOp.ArchiveEmployee(user.idUsuario);
                    }
                }
            }
        }
    }