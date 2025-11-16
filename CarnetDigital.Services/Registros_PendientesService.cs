using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using Microsoft.Extensions.Configuration;

namespace CarnetDigital.Services
{
    public class Registros_PendientesService : IRegistros_PendientesService
    {
        private readonly Registro_PendientesRepository  _registros;
        private readonly IConfiguration _config;

        public Registros_PendientesService (Registro_PendientesRepository registros, IConfiguration config)
        {
            _config = config;
            _registros = registros;
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var smtpSection = _config.GetSection("SmtpSettings");
            string servidor = smtpSection["Server"];
            int puerto = int.Parse(smtpSection["Port"]);
            string usuario = smtpSection["User"];
            string clave = smtpSection["Password"];
            bool enableSsl = bool.Parse(smtpSection["EnableSsl"]);

            using var smtp = new SmtpClient(servidor, puerto)
            {
                Credentials = new NetworkCredential(usuario, clave),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(usuario, "Sistema de Notificaciones"),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };

            mail.To.Add(destinatario);
            await smtp.SendMailAsync(mail);
        }

        public async Task<IEnumerable<Registros_Pendientes>> GetAllAsync()
        {
            try
            {
                return await _registros.GetAllAsync();
            }
            catch (Exception ex)
            {
                return new List<Registros_Pendientes>();
            }
        }

        public async Task<(string mensaje, int resultado)> EjecutarDecisionAsync(string identificacion, string decision)
        {
            try
            {
                return await _registros.EjecutarDecisionAsync(identificacion, decision);
            }
            catch (Exception ex)
            {
                return ($"Error al procesar la solicitud: {ex.Message}", 0);
            }
        }


    }
}
