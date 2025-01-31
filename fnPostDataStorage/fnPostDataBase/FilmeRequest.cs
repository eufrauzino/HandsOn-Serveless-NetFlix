using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fnPostDataBase
{
    internal class FilmeRequest
    {
        public string id { get { return Guid.NewGuid().ToString(); } } // gera um novo GUID para o id
        public string titulo { get; set; }
        public string ano { get; set; }
        public string video { get; set; }
        public string thumb { get; set; }
    }
}
