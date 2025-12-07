using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using Control_QR.Entities;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CarnetDigital.Services
{
    public class FuncionariosService : IFuncionariosService
    {
        private readonly FuncionariosRepository _repo;

        public FuncionariosService(FuncionariosRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool Resultado, string Mensaje)> RegistrarAsync(Funcionarios funcionario)
        {
            // ===== VALIDACIONES DE CAMPOS OBLIGATORIOS =====
            if (funcionario == null)
                return (false, "Los datos del funcionario son requeridos");

            if (string.IsNullOrWhiteSpace(funcionario.Nombre))
                return (false, "Debe ingresar el nombre");

            if (string.IsNullOrWhiteSpace(funcionario.Primer_Apellido))
                return (false, "Debe ingresar el primer apellido");

            if (string.IsNullOrWhiteSpace(funcionario.Segundo_Apellido))
                return (false, "Debe ingresar el segundo apellido");

            if (string.IsNullOrWhiteSpace(funcionario.Identificacion))
                return (false, "Debe ingresar la identificación");

            if (string.IsNullOrWhiteSpace(funcionario.CorreoInstitucional))
                return (false, "Debe ingresar el correo institucional");

            if (string.IsNullOrWhiteSpace(funcionario.Id_Tipo_Identificacion))
                return (false, "Debe seleccionar un tipo de identificación");

            if (string.IsNullOrWhiteSpace(funcionario.Id_Tipo_Funcionario))
                return (false, "Debe seleccionar un tipo de funcionario");

            if (string.IsNullOrWhiteSpace(funcionario.Id_Dependencia))
                return (false, "Debe seleccionar una dependencia");

            if (string.IsNullOrWhiteSpace(funcionario.FotoBase64))
                return (false, "Debe subir una foto del funcionario");


            // ===== VALIDACIÓN DE FECHA DE NACIMIENTO =====
            if (funcionario.Fecha_Nacimiento == DateTime.MinValue)
                return (false, "Formato de fecha inválido. Use una fecha válida.");

            if (funcionario.Fecha_Nacimiento < new DateTime(1753, 1, 1))
                return (false, "La fecha de nacimiento es inválida para el sistema.");

            var hoy = DateTime.Today;
            var edad = hoy.Year - funcionario.Fecha_Nacimiento.Year;

            if (funcionario.Fecha_Nacimiento.Date > hoy.AddYears(-edad))
                edad--;

            if (edad < 18)
                return (false, "El funcionario debe ser mayor de edad");


            // ===== VALIDACIÓN DE SOLO TEXTO =====
            if (!EsTexto(funcionario.Nombre))
                return (false, "El nombre solo permite letras y espacios");

            if (!EsTexto(funcionario.Primer_Apellido))
                return (false, "El primer apellido solo permite letras y espacios");

            if (!EsTexto(funcionario.Segundo_Apellido))
                return (false, "El segundo apellido solo permite letras y espacios");


            // ===== VALIDACIÓN DE CORREO =====
            if (!(funcionario.CorreoInstitucional.EndsWith("@cuc.cr") ||
                  funcionario.CorreoInstitucional.EndsWith("@cuc.ac.cr")))
            {
                return (false, "El correo debe terminar en @cuc.cr o @cuc.ac.cr");
            }


            // ===== VALIDACIÓN DE IDENTIFICACIÓN SEGÚN TIPO =====
            switch (funcionario.Id_Tipo_Identificacion)
            {
                case "CED":
                    if (!Regex.IsMatch(funcionario.Identificacion, @"^[0-9]{9}$"))
                        return (false, "La cédula debe tener exactamente 9 dígitos numéricos");
                    break;

                case "DIX":
                    if (!Regex.IsMatch(funcionario.Identificacion, @"^[0-9]{11,12}$"))
                        return (false, "El DIMEX debe tener entre 11 y 12 dígitos numéricos");
                    break;

                case "PAS":
                    if (!Regex.IsMatch(funcionario.Identificacion, @"^[A-Za-z0-9]{6,20}$"))
                        return (false, "El pasaporte debe tener entre 6 y 20 caracteres alfanuméricos");
                    break;

                default:
                    return (false, "Tipo de identificación no válido");
            }


            // ===== GENERAR CONTRASEÑA ANTES DE GUARDAR =====
            string contraseñaGenerada = GenerarContrasena();
            funcionario.Contrasena = HashPassword(contraseñaGenerada);

            // ===== GUARDAR EN BASE DE DATOS =====
            var resultado = await _repo.RegistrarAsync(funcionario);

            if (!resultado.Resultado)
            {
                if (resultado.Mensaje.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return (false, "Funcionario ya registrado");

                return resultado;
            }

            // ===== SOLO SI SE REGISTRÓ, SE ENVÍA EL CORREO =====
            var envio = await EnviarCorreo(funcionario.CorreoInstitucional, contraseñaGenerada);

            if (!envio.Enviado)
                return (false, "Funcionario registrado PERO ocurrió un error enviando el correo: " + envio.Error);

            return (true, "Registro exitoso. La contraseña fue enviada al correo institucional");
        }

        private bool EsTexto(string input)
        {
            return input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        private string GenerarContrasena()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@$#%&";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string HashPassword(string password)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(saltBytes);

            var salt = Convert.ToBase64String(saltBytes);
            using var sha = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha.ComputeHash(combined);

            return $"{salt}${Convert.ToBase64String(hash)}";
        }

        private async Task<(bool Enviado, string Error)> EnviarCorreo(string destino, string password)
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int port = 587;
                string usuario = "pruebacuc123@gmail.com";
                string claveApp = "wtbpxuglyxkgwvqq";

                using var cliente = new SmtpClient(smtpServer, port)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(usuario, claveApp),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 20000
                };

                string asunto = "Credenciales de acceso - Carnet Digital";

                string cuerpo = $@"
                <html>
                <body style='font-family:Arial, sans-serif; background-color:#f4f4f4; padding:20px;'>
                    <div style='max-width:600px; margin:auto; background:#ffffff; padding:25px; border-radius:10px; 
                                box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
        
                        <h2 style='color:#1a73e8; text-align:center; margin-top:0;'>
                            Credenciales de Acceso – Carnet Digital
                        </h2>

                        <p style='font-size:15px; color:#333;'>Estimado funcionario,</p>

                        <p style='font-size:15px; color:#333;'>
                            Su registro ha sido completado exitosamente en el 
                            <b>Sistema de Control de Entrada y Salida QR</b>.
                        </p>

                        <div style='background:#f1f7ff; padding:15px; border-left:4px solid #1a73e8; 
                                    margin:20px 0; border-radius:5px;'>
                            <p style='font-size:15px; margin:0; color:#1a73e8;'>
                                <b>Contraseña temporal:</b>
                            </p>
                            <h2 style='margin:10px 0 0 0; color:#0b5ed7; text-align:center; letter-spacing:1px;'>
                                {password}
                            </h2>
                        </div>

                        <p style='font-size:15px; color:#333;'>
                            Por motivos de seguridad, se le solicitara cambiar esta contraseña al ingresar por primera vez.
                        </p>

                        <br>

                        <p style='font-size:14px; color:#555;'>
                            Saludos cordiales,<br>
                            <b>Sistema de Control de Entrada y Salida QR</b>
                        </p>

                        <hr style='border:0; border-top:1px solid #ddd; margin:20px 0;'>

                        <p style='font-size:12px; color:#888; text-align:center;'>
                            Este es un mensaje automático, por favor no responder.
                        </p>
                    </div>
                </body>
                </html>";

                var mensaje = new MailMessage()
                {
                    From = new MailAddress(usuario, "Carnet Digital"),
                    Sender = new MailAddress(usuario),
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8
                };

                mensaje.ReplyToList.Add(usuario);
                mensaje.To.Add(destino);

                mensaje.Headers.Add("X-Priority", "3");
                mensaje.Headers.Add("X-MSMail-Priority", "Normal");
                mensaje.Headers.Add("Importance", "Normal");
                mensaje.Headers.Add("X-Originating-IP", "127.0.0.1");
                mensaje.Headers.Add("List-Unsubscribe", $"<{usuario}>");

                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        await cliente.SendMailAsync(mensaje);
                        return (true, null);
                    }
                    catch (SmtpException)
                    {
                        await Task.Delay(1500);
                    }
                }

                return (false, "El servidor rechazó el correo.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}

