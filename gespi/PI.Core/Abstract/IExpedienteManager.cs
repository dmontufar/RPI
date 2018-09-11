using PI.Common;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core.Abstract
{
    public interface IExpedienteManager : IManager<Expediente>
    {
        ExpedienteDeMarca GetExpedienteDeMarcas(string expediente);
        ExpedienteDeMarca GetExpedienteDeMarcasPorRegistro(int tipoDeRegistroId, int registro, string raya);
        ExpedienteDeMarca GetExpedienteDeMarcasPorId(int Id);
        ResultInfo BusquedaFonetica(int pageNumber, int pageSize, string textToSearch, string clases);
        ResultInfo BusquedaIdentica(int pageNumber, int pageSize, string textToSearch, string clases);

        ExpedienteDePatente GetExpedienteDePatentes(int tipoDeRegistroId, string solicitud);
        ExpedienteDePatente GetExpedienteDePatentesPorRegistro(int tipoDeRegistroId, int registro);
        ExpedienteDePatente GetExpedienteDePatentesPorId(int Id);
        ResultInfo BusquedaPatentesDsc(int pageNumber, int pageSize, string textToSearch, int? tipoDeRegistro);

        ExpedienteDAutor GetExpedienteDerechoAutor(string solicitud);
        ExpedienteDAutor GetExpedienteDerechoAutorPorRegistro(int registro);
        ExpedienteDAutor GetExpedienteDerechoAutorPorId(int Id);

        ExpedienteDeAnotaciones GetExpedienteDeAnotaciones(int tipoDeRegistroId, string solicitud);

        ResultInfo SaveEventoCronologicoDeMarcas(ResolucionCustomizada resolucion);
        ResultInfo SaveEventoCronologicoDeDA(ResolucionCustomizada resolucion);
        ResultInfo SaveEventoCronologicoDePatentes(ResolucionCustomizada resolucion);

        string getDOCResol(int cronologiaId);
        Expediente GetBaseDeExpedientePorId(int Id);
    }
}
