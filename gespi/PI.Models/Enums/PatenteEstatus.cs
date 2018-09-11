using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Enums
{
    public enum PatenteEstatus
    {
        NO_CAMBIA = 0,
        Solicitud_Ingresada = 59,
        Examen_Tecnico_de_Forma_Efectuado = 60,
        Requerimiento_de_forma_pendiente_de_notificar = 62,
        Admision_a_Tramite = 64,
        Edicto_Emitido_Pendiente_De_Entregar = 61,
        Publicada = 67,
        Orden_De_Pago_Pend_De_Notificacion = 95,
        Pago_Examen = 101,
        Requerimiento_Examen_de_Fondo = 98,
        Abandonado = 93,
        Desestimiento_Ley_57_2000 = 94,
        Registro_Efectuado = 88,
        Resolucion_De_Rechazo_Parcial = 71,
        Resolucion_De_Consecion = 72,
        Resolucion_De_Rechazo_Total = 99,

        Requerimiento_Forma_Notificado = 63,
        Edicto_Notificado = 66,
        Orden_De_Pago_Notificada = 68,
        Requerimiento_De_Examen_De_Fondo_Notificado = 97,
        Admision_Notificada = 116,
        Resolucion_De_Consecion_NOTIFICADA = 102,
        Resolucion_De_Rechazo_Total_Notificado = 96,
        Resolucion_De_Rechazo_Parcial_NOTIFICADA = 103,
        Solicitud_Denegada = 121,
        TRASPASO_DE_PATENTE = 110

    }
}
