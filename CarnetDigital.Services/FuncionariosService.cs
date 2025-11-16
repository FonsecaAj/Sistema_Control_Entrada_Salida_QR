using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using Control_QR.Entities;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

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
            if (string.IsNullOrWhiteSpace(funcionario.Nombre) ||
                string.IsNullOrWhiteSpace(funcionario.Primer_Apellido) ||
                string.IsNullOrWhiteSpace(funcionario.Segundo_Apellido) ||
                string.IsNullOrWhiteSpace(funcionario.Identificacion) ||
                string.IsNullOrWhiteSpace(funcionario.CorreoInstitucional))
            {
                return (false, "Este espacio no puede estar vacío");
            }

            if (!EsTexto(funcionario.Nombre) ||
                !EsTexto(funcionario.Primer_Apellido) ||
                !EsTexto(funcionario.Segundo_Apellido))
            {
                return (false, "Solo acepta letras y espacios");
            }

            if (string.IsNullOrWhiteSpace(funcionario.Id_Tipo_Identificacion))
                return (false, "Debe seleccionar un tipo de identificación");

            if (!(funcionario.CorreoInstitucional.EndsWith("@cuc.cr") ||
                  funcionario.CorreoInstitucional.EndsWith("@cuc.ac.cr")))
            {
                return (false, "Solo se permite @cuc.cr o @cuc.ac.cr");
            }

            if (string.IsNullOrWhiteSpace(funcionario.Id_Tipo_Funcionario))
                return (false, "Debe seleccionar un tipo de funcionario");

            if (string.IsNullOrWhiteSpace(funcionario.Id_Dependencia))
                return (false, "Debe seleccionar una dependencia");

            if (string.IsNullOrWhiteSpace(funcionario.FotoBase64))
                return (false, "Debe subir una foto");

            string contraseñaGenerada = GenerarContrasena();

            var envio = await EnviarCorreo(funcionario.CorreoInstitucional, contraseñaGenerada);

            if (!envio.Enviado)
                return (false, "Error enviando el correo: " + envio.Error);

            funcionario.Contrasena = HashPassword(contraseñaGenerada);

            var resultado = await _repo.RegistrarAsync(funcionario);

            if (!resultado.Resultado &&
                resultado.Mensaje.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Funcionario ya registrado");
            }

            if (resultado.Resultado)
                return (true, "Registro exitoso. La contraseña fue enviada correctamente al correo");

            return resultado;
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
                <body style='font-family:Arial; font-size:14px;'>
                    <p>Estimado funcionario,</p>
                    <p>Su registro ha sido completado exitosamente.</p>
                    <p><b>Contraseña temporal:</b></p>
                    <h2 style='color:#1a73e8'>{password}</h2>
                    <p>Por seguridad, cambie su contraseña antes de ingresar al sistema.</p>
                    <br>
                    <p>Saludos cordiales,<br>Carnet Digital</p>
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

