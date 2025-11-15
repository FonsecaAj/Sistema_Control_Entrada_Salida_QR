using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using Control_QR.Entities;
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
            // Validación MS2 (campos vacíos)
            if (string.IsNullOrWhiteSpace(funcionario.Nombre) ||
                string.IsNullOrWhiteSpace(funcionario.Primer_Apellido) ||
                string.IsNullOrWhiteSpace(funcionario.Segundo_Apellido) ||
                string.IsNullOrWhiteSpace(funcionario.Identificacion) ||
                string.IsNullOrWhiteSpace(funcionario.CorreoInstitucional) ||
                string.IsNullOrWhiteSpace(funcionario.Contrasena))
            {
                return (false, "Este espacio no puede estar vacío");
            }

            // Validación MS1 (solo letras)
            if (!EsTexto(funcionario.Nombre) ||
                !EsTexto(funcionario.Primer_Apellido) ||
                !EsTexto(funcionario.Segundo_Apellido))
            {
                return (false, "Solo acepta letras y espacios en blanco");
            }

            // Validación MS3 – Tipo Identificación
            if (string.IsNullOrWhiteSpace(funcionario.Id_Tipo_Identificacion))
            {
                return (false, "Debe seleccionar una de los tipos de Identificación de la lista");
            }

            // MS5 – Correo institucional
            if (!(funcionario.CorreoInstitucional.EndsWith("@cuc.cr") ||
                  funcionario.CorreoInstitucional.EndsWith("@cuc.ac.cr")))
            {
                return (false, "Solo permite @cuc.cr o @cuc.ac.cr");
            }

            // MS6 – Tipo funcionario requerido
            if (string.IsNullOrWhiteSpace(funcionario.Id_Tipo_Funcionario))
                return (false, "Debe seleccionar uno de los tipos de funcionarios de la lista");

            // MS7 – Dependencia requerida
            if (string.IsNullOrWhiteSpace(funcionario.Id_Dependencia))
                return (false, "Debe seleccionar una de las dependencias de la lista");

            // MS10 – Foto requerida
            if (string.IsNullOrWhiteSpace(funcionario.FotoBase64))
                return (false, "Debe subir una foto suya para registrarse");

            // cifrado de contraseña
            funcionario.Contrasena = HashPassword(funcionario.Contrasena);

            var resultado = await _repo.RegistrarAsync(funcionario);

            // Si ya existe → MS8
            if (!resultado.Resultado && resultado.Mensaje.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Funcionario ya registrado");
            }

            // Registro exitoso → MS9
            if (resultado.Resultado)
            {
                return (true, "Registro exitoso");
            }

            return resultado;
        }

        private bool EsTexto(string input)
        {
            return input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        private string HashPassword(string password)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            var salt = Convert.ToBase64String(saltBytes);

            using var sha = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha.ComputeHash(combined);
            var hashString = Convert.ToBase64String(hash);

            return $"{salt}${hashString}";
        }
    }
}


