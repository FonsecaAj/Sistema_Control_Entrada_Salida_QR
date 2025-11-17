using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
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
        private readonly Registros_PendientesRepository _registrospendientesRepository;
        private readonly IConfiguration _config;

        public Registros_PendientesService(Registros_PendientesRepository registrospendientesRepository, IConfiguration config)
        {
            _config = config;
            _registrospendientesRepository = registrospendientesRepository;

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
                return await _registrospendientesRepository.GetAllAsync();
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
                return await _registrospendientesRepository.EjecutarDecisionAsync(identificacion, decision);
            }
            catch (Exception ex)
            {
                return ($"Error al procesar la solicitud: {ex.Message}", 0);
            }
        }

        public async Task<Registros_Pendientes> RegistrarUsuarioAsync(Registros_Pendientes registro)
        {
            //if (string.IsNullOrWhiteSpace(registro.Nombre))
            //{

            //    registro.Mensaje = " El campo nombre es obligatorio";
            //    return registro;

            //}

            //if (string.IsNullOrWhiteSpace(registro.Primer_Apellido))
            //{
            //    registro.Mensaje = "El Primer Apellido es obligatorio";
            //    return registro;
            //}

            //if (string.IsNullOrWhiteSpace(registro.Segundo_Apellido))
            //{
            //    registro.Mensaje = "El Segundo Apellido es obligatorio";
            //    return registro;
            //}

            //if (string.IsNullOrWhiteSpace(registro.Correo_Institucional))
            //{

            //    registro.Mensaje = "El correo es obligatorio";
            //    return registro;

            //}

            //if (string.IsNullOrWhiteSpace(registro.ID_Tipo_Identificacion))
            //{

            //    registro.Mensaje = "El tipo de Identificación es un campo obligatorio ";
            //    return registro;

            //}

            //if (string.IsNullOrWhiteSpace(registro.Identificacion))
            //{
            //    registro.Mensaje = "La identificación es un campo obligatorio";
            //    return registro;
            //}

            //if (string.IsNullOrWhiteSpace(registro.Id_Carrera))
            //{

            //    registro.Mensaje = "Las carreras o programas es un campo obligatorio";
            //    return registro;

            //}

            //if (string.IsNullOrWhiteSpace(registro.Contrasena))
            //{

            //    registro.Mensaje = "La contraseña es un campo obligatorio";
            //    return registro;

            //}


            if (string.IsNullOrWhiteSpace(registro.Nombre) || string.IsNullOrWhiteSpace(registro.Primer_Apellido) || string.IsNullOrWhiteSpace(registro.Correo_Institucional) || string.IsNullOrWhiteSpace(registro.ID_Tipo_Identificacion) || string.IsNullOrWhiteSpace(registro.Identificacion) || string.IsNullOrWhiteSpace(registro.Id_Carrera) || string.IsNullOrWhiteSpace(registro.Contrasena))
            {
                registro.Mensaje = "Todos los campos son obligatorios";
                return registro;
            }

            if (registro.Correo_Institucional.Length > 100)
            {
                registro.Mensaje = "El correo institucional no puede superar los 100 caracteres.";
                return registro;
            }

            if (!registro.Correo_Institucional.EndsWith("@cuc.cr"))
            {
                registro.Mensaje = "Solo se permiten correos institucionales del CUC.";
                return registro;
            }

            if (registro.Foto == null || registro.Foto.Length == 0)
            {
                registro.Mensaje = "Debe subir una foto para completar el registro.";
                return registro;
            }

            registro.Contrasena = HashPassword(registro.Contrasena);

            var resultado = await _registrospendientesRepository.RegistrarUsuarioAsync(registro);

            return resultado;
        }

        private string HashPassword(string password)
        {
            // Generar SALT
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            var salt = Convert.ToBase64String(saltBytes);

            // Crear hash SHA256(password + salt)
            using var sha = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha.ComputeHash(combined);
            var hashString = Convert.ToBase64String(hash);

            // Guardar en formato: SALT$HASH
            return $"{salt}${hashString}";
        }


    }
}
