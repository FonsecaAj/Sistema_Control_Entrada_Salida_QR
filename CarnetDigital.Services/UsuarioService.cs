using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services
{
    public class UsuarioService: IUsuarioService
    {

        private readonly UsuarioRepository _usuariosRepository;
        private readonly IConfiguration _config;

        public UsuarioService(UsuarioRepository usuariosRepository, IConfiguration config)
        {
            _usuariosRepository = usuariosRepository;
            _config = config;
        }


        private string PlantillaCorreo(string nombre, string mensajePrincipal, DateTime fechavencimiento)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f6f9;'>
                    <div style='max-width: 650px; margin: auto; background: white; padding: 25px; border-radius: 12px;
                                box-shadow: 0 3px 12px rgba(0,0,0,0.12);'>

                        <h2 style='color: #002D74; text-align:center; margin-bottom:25px;'>
                            Aviso Importante sobre su carnet digital
                        </h2>

                        <p style='font-size: 15px; color:#333;'>
                            Hola <strong>{nombre}</strong>,
                        </p>

                        <p style='font-size: 15px; color:#333;'>{mensajePrincipal}</p>

                        <div style='background:#e6efff; padding:15px; border-left:5px solid #F5333F; border-radius:6px; margin-top:15px;'>
                            <p style='font-size: 16px; color:#F5333F; margin:0;'>
                                <strong> Fecha de Vencimiento: {fechavencimiento:dd/MM/yyyy}</strong>
                            </p>
                        </div>

                        <p style='margin-top:20px; font-size: 15px; color:#444;'>
                            Recuerde que debe <strong>asistir al Departamento de Registro</strong> 
                            para renovar su carnet, según las fechas indicadas en el 
                            <strong>cronograma oficial</strong>.
                        </p>

                        <p style='margin-top:15px; font-size: 15px; color:#444;'>
                            Es importante completar este trámite a tiempo para evitar inconvenientes en el acceso a los servicios institucionales.
                        </p>

                        <p style='margin-top:35px; font-size:12px; color:#777; text-align:center;'}}>
                            Este es un mensaje automático. Por favor no responder a este correo.
                        </p>
                    </div>
                </div>";
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



        public async Task<Usuarios> LoginAsync(string correoInstitucional, string contrasena)
        {
            
            if (string.IsNullOrWhiteSpace(correoInstitucional))
            {
                return new Usuarios
                {
                    Correo_Institucional = correoInstitucional,
                    Contrasena = contrasena,
                    Mensaje = "El espacio de correo institucional no puede estar vacío"
                };
            }

            if (string.IsNullOrWhiteSpace(contrasena))
            {
                return new Usuarios
                {
                    Correo_Institucional = correoInstitucional,
                    Contrasena = contrasena,
                    Mensaje = "El espacio de la contraseña no puede estar vacío"
                };

            }

            
            var resultado = await _usuariosRepository.LoginAsync(correoInstitucional, contrasena);


            if (resultado.Alerta_Vencimiento == "Carnet vence en 30 días")
            {
                string cuerpo = PlantillaCorreo(
                    resultado.NombreCompleto,
                    "Su carnet digital vencerá en 30 días.",
                    resultado.FechaVencimiento.Value
                );

                await EnviarCorreoAsync(
                    resultado.Correo_Institucional,
                    "Aviso: su carnet vencerá en 30 días",
                    cuerpo
                );
            }
            else if (resultado.Alerta_Vencimiento == "Carnet vence en 1 día")
            {
                string cuerpo = PlantillaCorreo(
                    resultado.NombreCompleto,
                    "Su carnet digital vencerá mañana.",
                    resultado.FechaVencimiento.Value
                );

                await EnviarCorreoAsync(
                    resultado.Correo_Institucional,
                    "Aviso: su carnet vencerá mañana",
                    cuerpo
                );
            }

            return new Usuarios
            {
                Identificacion = resultado.Identificacion,
                Correo_Institucional = resultado.Correo_Institucional,
                Contrasena = resultado.Contrasena,
                NombreCompleto = resultado.NombreCompleto,
                Rol = resultado.Rol,
                FechaVencimiento = resultado.FechaVencimiento,
                ID_Carrera = resultado.ID_Carrera,
                ID_TipoEstudiante = resultado.ID_TipoEstudiante,

                Id_Dependencia = resultado.Id_Dependencia,
                Id_Tipo_Funcionario = resultado.Id_Tipo_Funcionario,
                Mensaje = resultado.Mensaje,
                
            };

        }

    }
}
