using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarnetDigital.Services
{
    public class Registros_PendientesService : IRegistros_PendientesService
    {
        private readonly Registros_PendientesRepository _registrospendientesRepository;

        public Registros_PendientesService(Registros_PendientesRepository registrospendientesRepository)
        {

            _registrospendientesRepository = registrospendientesRepository;

        }

        public async Task<Registros_Pendientes> RegistrarUsuarioAsync(Registros_Pendientes registro)
        {

            // ---------- MS2: campos obligatorios ----------

            if (string.IsNullOrWhiteSpace(registro.Nombre) || string.IsNullOrWhiteSpace(registro.Primer_Apellido) || string.IsNullOrWhiteSpace(registro.Segundo_Apellido) || string.IsNullOrWhiteSpace(registro.Correo_Institucional) ||  string.IsNullOrWhiteSpace(registro.Identificacion) ||  string.IsNullOrWhiteSpace(registro.Contrasena))
            {
                registro.Mensaje = "El campo es obligatorio no puede estar vacío";
                return registro;
            }

            // ---------- MS1: solo letras y espacios ----------

            if (!Regex.IsMatch(registro.Nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                registro.Mensaje = "Solo acepta letras y espacios en blanco";
                return registro;
            }
            if (!Regex.IsMatch(registro.Primer_Apellido, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                registro.Mensaje = "Solo acepta letras y espacios en blanco";
                return registro;
            }
            if (!Regex.IsMatch(registro.Segundo_Apellido, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                registro.Mensaje = "Solo acepta letras y espacios en blanco";
                return registro;
            }

            // ---------- MS3: tipo de identificación ----------

            if (registro.ID_Tipo_Identificacion == "0" || string.IsNullOrEmpty(registro.ID_Tipo_Identificacion))
            {
                registro.Mensaje = "Debe seleccionar un tipo de identificación";
                return registro;
            }

            // ---------- MS4: identificación solo números ----------

            if (!Regex.IsMatch(registro.Identificacion, @"^[0-9]+$"))
            {
                registro.Mensaje = "Solo permite números";
                return registro;
            }

            // ---------- MS5: debe seleccionar carrera ----------

            if (registro.Id_Carrera == "0" || string.IsNullOrEmpty(registro.Id_Carrera))
            {
                registro.Mensaje = "Debe seleccionar una de las carreras o programas"; 
                return registro;
            }

            // ---------- MS6: correo institucional ----------

            if (!(registro.Correo_Institucional.EndsWith("@cuc.cr") ||
                  registro.Correo_Institucional.EndsWith("@cuc.ac.cr")))
            {
                registro.Mensaje = "Solo permite correos con dominio @cuc.cr o @cuc.ac.cr";
                return registro;
            }

            // ---------- MS7: contraseña mínima 8 caracteres y fuerte ----------

            bool tieneLetra = Regex.IsMatch(registro.Contrasena, @"[A-Za-z]");
            bool tieneNumero = Regex.IsMatch(registro.Contrasena, @"[0-9]");
            bool tieneEspecial = Regex.IsMatch(registro.Contrasena, @"[\W_]");

            if (registro.Contrasena.Length < 8 || !tieneLetra || !tieneNumero || !tieneEspecial)
            {
                registro.Mensaje = "Debe tener un mínimo de 8 caracteres entre letras, números y caracteres especiales";
                return registro;
            }

            // ---------- MS8: foto requerida ----------

            if (registro.Foto == null || registro.Foto.Length == 0)
            {
                registro.Mensaje = "Debe subir una foto suya para registrarse";
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
