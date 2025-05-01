using System;

namespace MilyUnaNochesService.Utilities
{
    public class EmployeeData
    {
        public int idAcceso { get; set; }
        public int idUsuario { get; set; }
        public int idEmpleado { get; set; }
        public string nombre { get; set; }
        public string primerApellido { get; set; }
        public string segundoApellido { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }

        public string tipoEmpleado { get; set; }
        public string calle { get; set; }
        public string numero { get; set; }
        public string codigoPostal { get; set; }
        public string ciudad { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is EmployeeData other)
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
