using System;

namespace MilyUnaNochesService.Utilities
{
    public class ClientData
    {

        public int idUsuario { get; set; }
        public string nombre { get; set; }
        public string primerApellido { get; set; }
        public string segundoApellido { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ClientData other)
            {
                return idUsuario == other.idUsuario &&
                    nombre == other.nombre && primerApellido == other.primerApellido &&
                    segundoApellido == other.segundoApellido && correo == other.correo &&
                    telefono == other.telefono;
            }
            return false;
        }

    }
}
