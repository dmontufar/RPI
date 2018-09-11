using PI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core.Abstract
{
    public interface IEnumManager
    {
        ResultInfo GetLeyes();
        ResultInfo GetModulos();
        //ResultInfo GetTipoDeDatos();
        ResultInfo GetTiposDePatente();
        ResultInfo GetTiposDeObra();
        ResultInfo GetTiposDeRegistroDeMarca();
        ResultInfo GetTiposDeAnotaciones();
        ResultInfo GetPaises();
        ResultInfo GetClasificaciones();
    }
}
