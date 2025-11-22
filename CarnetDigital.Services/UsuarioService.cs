using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services
{
    public class UsuarioService: IUsuarioService
    {

        private readonly UsuarioRepository _usuariosRepository;

        public UsuarioService(UsuarioRepository usuariosRepository)
        {
            _usuariosRepository = usuariosRepository;
        }

        public async Task<(bool Resultado, string Mensaje)> CambiarContrasenaAsync(string identificacion, string actual, string nueva)
        {
            // 4. Validar que los espacios no estén en blanco
            if (string.IsNullOrWhiteSpace(actual) || string.IsNullOrWhiteSpace(nueva))
                return (false, "El espacio de la contraseña no puede quedar en blanco");

            // 2. Validar que la contraseña actual no sea igual a la nueva
            if (actual == nueva)
                return (false, "La contraseña ingresada no puede ser igual a la actual");

            // 5. Validar formato y longitud (mínimo 8 caracteres con letras, números y especiales)
            if (nueva.Length < 8 ||
                !nueva.Any(char.IsLetter) ||
                !nueva.Any(char.IsDigit) ||
                !nueva.Any(c => "!@#$%^&*()_-+=<>?/{}~|".Contains(c)))
            {
                return (false, "Debe tener un mínimo de 8 caracteres entre letras, números y caracteres especiales");
            }

            // Llamar al repositorio (SP)
            var resultado = await _usuariosRepository.CambiarContrasenaAsync(identificacion, actual, nueva);

            // 6. Confirmación exitosa
            if (resultado.Resultado)
                return (true, "Cambio de Contraseña Exitoso");

            return resultado;
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

            if (resultado.Rol == "FUN" && resultado.PrimerIngreso)
            {
                resultado.Mensaje = "CAMBIAR_CONTRASENA";
                return resultado;
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
                PrimerIngreso = resultado.PrimerIngreso

            };

        }

    }
}
