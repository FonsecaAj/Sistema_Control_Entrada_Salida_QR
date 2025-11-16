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
