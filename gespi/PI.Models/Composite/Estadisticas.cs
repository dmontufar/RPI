using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class ExpedientesByArea
    {
        public int DAIngreso { get; set; }
        public int SignosIngreso { get; set; }
        public int PatentesIngreso { get; set; }
        public int DAPublicacion { get; set; }
        public int SignosPublicacion { get; set; }
        public int PatentesPublicacion { get; set; }
        public int DAConTitulo { get; set; }
        public int SignosConTitulo { get; set; }
        public int PatentesConTitulo { get; set; }
    }

    public class IngresoExpedientesMensual
    {
        public int ModuloId { get; set; }
        public int MES { get; set; }
        public int Conteo { get; set; }
    }
}
