using PI.Common;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Abstract
{
    public interface IExpedienteRepository : IRepository<Expediente>
    {
        Expediente GetBaseDeExpedientePorId(int Id);
        string getDOCResol(int cronologiaId);

        ExpedienteDeMarca GetExpedienteDeMarcas(string numero);
        ExpedienteDeMarca GetExpedienteDeMarcasPorRegistro(int tipoDeRegistroId, int registro, string raya);
        ExpedienteDeMarca GetExpedienteDeMarcasPorId(int Id);
        PagedList BusquedaFonetica(int pageNumber, int pageSize, string textToSearch, string clases);
        PagedList BusquedaIdentica(int pageNumber, int pageSize, string textToSearch, string clases);        

        ExpedienteDePatente GetExpedienteDePatentes(int tipoDeRegistroId, string numero);
        ExpedienteDePatente GetExpedienteDePatentesPorRegistro(int tipoDeRegistroId, int registro);
        ExpedienteDePatente GetExpedienteDePatentesPorId(int Id);
        PagedList BusquedaPatentesDsc(int pageNumber, int pageSize, string textToSearch, int? tipoDeRegistro);

        ExpedienteDAutor GetExpedienteDerechoAutor(string solicitud);
        ExpedienteDAutor GetExpedienteDerechoAutorPorRegistro(int registro);
        ExpedienteDAutor GetExpedienteDerechoAutorPorId(int Id);

        //Expediente AddExpediente(Expediente entity);
        //Expediente UpdateExpediente(Expediente entity);
        //Expediente AnularExpediente(Expediente entity);

        ExpedienteDeAnotaciones GetExpedienteDeAnotaciones(int tipoDeRegistroId, string numero);

        ResultInfo SaveEventoCronologicoDeMarcas(ResolucionCustomizada resolucion);
        ResultInfo SaveEventoCronologicoDeDA(ResolucionCustomizada resolucion);
        ResultInfo SaveEventoCronologicoDePatentes(ResolucionCustomizada resolucion);
    }
}
