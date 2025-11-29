using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarnetDigital.Services
{
    public class Encargados_LegalesService : IEncargados_LegalesService
    {
        private readonly Encargados_LegalesRepository _encargados_LegalesRepository;

        public Encargados_LegalesService(Encargados_LegalesRepository encargados_LegalesRepository)
        {
            _encargados_LegalesRepository = encargados_LegalesRepository;
        }

        public async Task<IEnumerable<Entities.Tipos_Identificacion>> ObtenerTodosLosTiposIdentificacion()
        {
            return await _encargados_LegalesRepository.ObtenerTodosLosTiposIdentificacion();
        }

        public async Task<IEnumerable<Entities.Parentescos>> ObtenerTodosLosParentescos()
        {
            return await _encargados_LegalesRepository.ObtenerTodosLosParentescos();
        }

        public async Task<(string, bool)> RegistrarEncargadoLegalAsync(Encargados_Legales encargado)
        {
            if (encargado == null)

                return ("Los datos del funcionario son requeridos", false);

            if (string.IsNullOrWhiteSpace(encargado.Nombre))

                return ("Debe ingresar el nombre", false);

            if (string.IsNullOrWhiteSpace(encargado.Primer_Apellido))

                return ("Debe ingresar el primer apellido", false);

            if (string.IsNullOrWhiteSpace(encargado.Segundo_Apellido))

                return ("Debe ingresar el segundo apellido", false);

            if (string.IsNullOrWhiteSpace(encargado.ID_Tipo_Identificacion))

                return ("Debe seleccionar un tipo de identificación", false);

            if (string.IsNullOrWhiteSpace(encargado.Identificacion))
                return ("Debe ingresar la identificación", false);

            if (string.IsNullOrWhiteSpace(encargado.Telefono))
                return ("Debe ingresar el número de telefono", false);

            var parentescos = await _encargados_LegalesRepository.ObtenerTodosLosParentescos();

            if (!parentescos.Any(p => p.Id_Parentesco == encargado.Id_Parentesco))
            {
                return ("Debe seleccionar uno de los parentescos de la lista", false);
            }

            if (encargado.Foto == null || encargado.Foto.Length == 0)
                return ("Debe subir una foto del encargado", false);

            bool SoloLetras(string valor) =>
                !string.IsNullOrWhiteSpace(valor) &&
                valor.All(c => char.IsLetter(c) || c == ' ');

            if (!SoloLetras(encargado.Nombre))
                return ("El nombre solo permite letras y espacios", false);

            if (!SoloLetras(encargado.Primer_Apellido))
                return ("El primer apellido solo permite letras y espacios", false);

            if (!SoloLetras(encargado.Segundo_Apellido))
                return ("El segundo apellido solo permite letras y espacios", false);

            switch (encargado.ID_Tipo_Identificacion)

            {
                case "CED":

                    if (!Regex.IsMatch(encargado.Identificacion, @"^\[0-9]{9}$"))

                        return ("La cédula debe tener exactamente 9 dígitos numéricos", false);

                    break;



                case "DIX":

                    if (!Regex.IsMatch(encargado.Identificacion, @"^\[0-9]{11,12}$"))

                        return ("El DIMEX debe tener entre 11 y 12 dígitos numéricos", false);

                    break;



                case "PAS":

                    if (!Regex.IsMatch(encargado.Identificacion, @"^\[A-Za-z0-9]{6,20}$"))

                        return ("El pasaporte debe tener entre 6 y 20 caracteres alfanuméricos", false);

                    break;



                default:

                    return ("Tipo de identificación no válido", false);

            }

            //if (encargado == null ||
            //    string.IsNullOrWhiteSpace(encargado.Identificacion_Estudiante) ||
            //    string.IsNullOrWhiteSpace(encargado.Identificacion) ||
            //    string.IsNullOrWhiteSpace(encargado.Nombre) ||
            //    string.IsNullOrWhiteSpace(encargado.Primer_Apellido) ||
            //    string.IsNullOrWhiteSpace(encargado.Telefono) ||
            //    string.IsNullOrWhiteSpace(encargado.ID_Tipo_Identificacion) ||
            //    encargado.Id_Parentesco == 0 ||
            //    encargado.Foto == null)
            //{
            //    return ("Debe completar todos los espacios para registrarse", false);
            //}

            //bool SoloLetras(string valor) =>
            //    valor.All(c => char.IsLetter(c) || c == ' ');

            //if (!SoloLetras(encargado.Nombre) ||
            //    !SoloLetras(encargado.Primer_Apellido) ||
            //    (!string.IsNullOrWhiteSpace(encargado.Segundo_Apellido) && !SoloLetras(encargado.Segundo_Apellido)))
            //{
            //    return ("Solo acepta letras y espacios en blanco", false);
            //}
            //bool SoloNumeros(string valor) =>
            //    valor.All(char.IsDigit);

            //if (!SoloNumeros(encargado.Identificacion) || !SoloNumeros(encargado.Telefono))
            //{
            //    return ("Solo permite numeros", false);
            //}
            //var tipos = await _encargados_LegalesRepository.ObtenerTodosLosTiposIdentificacion();

            //if (!tipos.Any(t => t.ID_Tipo_Identificacion == encargado.ID_Tipo_Identificacion))
            //{
            //    return ("Debe seleccionar uno de los tipos de identificacion", false);
            //}
            //var parentescos = await _encargados_LegalesRepository.ObtenerTodosLosParentescos();

            //if (!parentescos.Any(p => p.Id_Parentesco == encargado.Id_Parentesco))
            //{
            //    return ("Debe seleccionar uno de los parentescos de la lista", false);
            //}
            //if (encargado.Foto == null || encargado.Foto.Length == 0)
            //{
            //    return ("Debe subir una foto suya para registrarse", false);
            //}

            var mensajeSP = await _encargados_LegalesRepository.RegistrarEncargadoLegalAsync(encargado);

            return mensajeSP;
        }

    }
}
