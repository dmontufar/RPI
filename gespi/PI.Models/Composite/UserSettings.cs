using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class UserSettings
    {
        public Usuario Usuario { get; set; }
        public string access_token { get; set; }
    }

    public class UsuarioPublicoSettingsX
    {
        public UsuarioPublico Usuario { get; set; }
        public string access_token { get; set; }
    }

}
