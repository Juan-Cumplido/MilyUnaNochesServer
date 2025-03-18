using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesService.Logic {
    public class Address {
        public int IdDireccion { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string CodigoPostal { get; set; }
        public string Ciudad { get; set; }
    }
}
