using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Enums
{
    public enum DAExpedienteEstatus
    {
        Solicitud_Ingresada = 130,
        Rechazo_de_plano = 131,
        Se_declara_con_Lugar = 132,
        Suspenso_x_Requerimiento = 133,
        Levantar_Suspension_x_Sentencia = 134,
        Registrada = 135,
        Recurso_de_Revocatoria = 136,
        Elevando_Recurso_de_Revocatoria = 137,
        Por_recibido_MINECO_a_Registro = 138,
        Por_recibido_MINECO_a_Archivo = 139,
        Pendiente_de_Registrar = 140,
        Notificado = 141,
        Memorial_Pendiente_de_Operar = 142,
    }
}
