using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Abstract
{
    public interface IEnumRepository 
    {
        IEnumerable<Leyes> GetLeyes();
        IEnumerable<Modulo> GetModulos();
        //IEnumerable<TipoDeDato> GetTipoDeDatos();
        IEnumerable<TiposDeRegistro> GetTiposDePatente();
        IEnumerable<TiposDeRegistro> GetTiposDeObra();
        IEnumerable<TiposDeRegistro> GetTiposDeRegistroDeMarca();
        IEnumerable<TiposDeRegistro> GetTiposDeAnotaciones();
        IEnumerable<Pais> GetPaises();
        IEnumerable<Clasificacion> GetClasificaciones();
    }
}
