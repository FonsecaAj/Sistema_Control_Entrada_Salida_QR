using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using Control_QR.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            // ===== VALIDACIONES DE CAMPOS OBLIGATORIOS =====

            if (registro == null)
            {
                registro.Mensaje = "Los datos del funcionario son requeridos";
                return registro;
            }
            if (string.IsNullOrWhiteSpace(registro.Nombre))
            {
                registro.Mensaje = "Debe ingresar el nombre";
                return registro;
            }
            if (string.IsNullOrWhiteSpace(registro.Primer_Apellido))
            {
                registro.Mensaje = "Debe ingresar el primer apellido";
                return registro;
            }
            if (string.IsNullOrWhiteSpace(registro.Segundo_Apellido))
            {
                registro.Mensaje = "Debe ingresar el segundo apellido";
                return registro;
            }
            if (string.IsNullOrWhiteSpace(registro.Correo_Institucional))
            {
                registro.Mensaje = "Debe ingresar el correo institucional";
                return registro;
            }
            if (string.IsNullOrWhiteSpace(registro.Identificacion))
            {
                registro.Mensaje = "Debe ingresar la identificación";
                return registro;
            }
            if (string.IsNullOrWhiteSpace(registro.Contrasena))
            {
                registro.Mensaje = "Debe ingresar la contraseña";
                return registro;
            }
            if (registro.ID_Tipo_Identificacion == "0" || string.IsNullOrEmpty(registro.ID_Tipo_Identificacion))
            {
                registro.Mensaje = "Debe seleccionar un tipo de identificación";
                return registro;
            }
            if (registro.Id_Carrera == "0" || string.IsNullOrEmpty(registro.Id_Carrera))
            {
                registro.Mensaje = "Debe seleccionar una de las carreras o programas";
                return registro;
            }
            if (registro.Foto == null || registro.Foto.Length == 0)
            {
                registro.Mensaje = "Debe subir una foto suya para registrarse";
                return registro;
            }
            if (registro.Fecha_Nacimiento == DateTime.MinValue)
            {
                registro.Mensaje = "Debe ingresar una fecha de nacimiento válida";
                return registro;
            }


            // ===== VALIDACIÓN DE SOLO TEXTO =====

            if (!Regex.IsMatch(registro.Nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                registro.Mensaje = "El nombre solo permite letras y espacios";
                return registro;
            }
            if (!Regex.IsMatch(registro.Primer_Apellido, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                registro.Mensaje = "El primer apellido solo permite letras y espacios";
                return registro;
            }
            if (!Regex.IsMatch(registro.Segundo_Apellido, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                registro.Mensaje = "El segundo apellido solo permite letras y espacios";
                return registro;
            }

            // ===== VALIDACIÓN DE IDENTIFICACIÓN SEGÚN TIPO =====
            switch (registro.ID_Tipo_Identificacion)
            {
                case "CED":
                    if (!Regex.IsMatch(registro.Identificacion, @"^[0-9]{9}$"))
                    {
                        registro.Mensaje = "La cédula debe tener exactamente 9 dígitos numéricos";
                        return registro;
                    }                        
                    break;

                case "DIX":
                    if (!Regex.IsMatch(registro.Identificacion, @"^[0-9]{11,12}$"))
                    {
                        registro.Mensaje = "El DIMEX debe tener entre 11 y 12 dígitos numéricos";
                        return registro;
                    }
                    break;

                case "PAS":
                    if (!Regex.IsMatch(registro.Identificacion, @"^[A-Za-z0-9]{6,20}$"))
                    {
                        registro.Mensaje = "El pasaporte debe tener entre 6 y 20 caracteres alfanuméricos";
                        return registro;
                    }
                    break;

                default:

                    registro.Mensaje = "Tipo de identificación no válido";
                    return registro;
            }

            // ===== VALIDACIÓN DE CORREO =====
            if (!(registro.Correo_Institucional.EndsWith("@cuc.cr")))
            {
                registro.Mensaje = "El correo debe terminar en @cuc.cr";
                return registro;
            }

            // ===== VALIDACIÓN DE CONTRASEÑA =====

            bool tieneLetra = Regex.IsMatch(registro.Contrasena, @"[A-Za-z]");
            bool tieneNumero = Regex.IsMatch(registro.Contrasena, @"[0-9]");
            bool tieneEspecial = Regex.IsMatch(registro.Contrasena, @"[\W_]");

            if (registro.Contrasena.Length < 8 || !tieneLetra || !tieneNumero || !tieneEspecial)
            {
                registro.Mensaje = "Debe tener un mínimo de 8 caracteres entre letras, números y caracteres especiales";
                return registro;
            }

            registro.Contrasena = HashPassword(registro.Contrasena);

            var resultado = await _registrospendientesRepository.RegistrarUsuarioAsync(registro);

            // ---------- MS9: Usuario ya Registrado ----------

            if (resultado.Mensaje == "Ya existe un usuario registrado con esa identificación" || resultado.Mensaje == "El correo institucional ya está registrado")
            {
                resultado.Mensaje = "Usuario ya registrado";
                return resultado;
            }


            // ---------- MS10: Registro Existoso ----------

            resultado.Mensaje = "Registro exitoso. Espere el correo de activación de perfil para ingresar al sistema";
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
