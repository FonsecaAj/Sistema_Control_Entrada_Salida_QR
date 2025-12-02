using CarnetDigital.Entities;
using CarnetDigital.Repository;
using CarnetDigital.Services.Abstract;
using Control_QR.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarnetDigital.Services
{
    public class EncargadoTemporalService : IEncargadoTemporalService
    {
        private readonly EncargadosTemporalesRepository _encargados_TemporalesRepository;

        public EncargadoTemporalService(EncargadosTemporalesRepository encargados_TemporalesRepository)
        {
            _encargados_TemporalesRepository = encargados_TemporalesRepository;
        }

        public async Task<IEnumerable<Entities.Tipos_Identificacion>> ObtenerTodosLosTiposIdentificacion()
        {
            return await _encargados_TemporalesRepository.ObtenerTodosLosTiposIdentificacion();
        }

        public async Task<IEnumerable<Entities.Parentescos>> ObtenerTodosLosParentescos()
        {
            return await _encargados_TemporalesRepository.ObtenerTodosLosParentescos();
        }

        public async Task<(string, bool)> RegistrarEncargadoTemporalAsync(Encargados_Temporales encargado)
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
            var id = encargado.Identificacion.Trim();
            var tipo = encargado.ID_Tipo_Identificacion.ToUpper();

            switch (tipo)
            {
                case "CED":
                    if (!id.All(char.IsDigit))
                        return ("La identificación cédula solo puede contener dígitos numéricos.", false);

                    if (id.Length != 9)
                        return ("La identificación cédula debe tener exactamente 9 dígitos numéricos.", false);
                    break;

                case "DIX":
                    if (!id.All(char.IsDigit))
                        return ("La identificación DIMEX solo puede contener dígitos numéricos.", false);

                    if (id.Length < 11 || id.Length > 12)
                        return ("La identificación DIMEX debe tener entre 11 y 12 dígitos numéricos.", false);
                    break;

                case "PAS":
                    if (!id.All(char.IsLetterOrDigit))
                        return ("La identificación pasaporte solo puede contener caracteres alfanuméricos.", false);

                    if (id.Length < 6 || id.Length > 22)
                        return ("La identificación pasaporte debe tener entre 6 y 22 caracteres alfanuméricos.", false);
                    break;

                default:
                    return ("Tipo de identificación no válido.", false);
            }

            if (string.IsNullOrWhiteSpace(encargado.Telefono))
                return ("Debe ingresar el número de telefono", false);

            if (!Regex.IsMatch(encargado.Telefono, @"^[0-9]{8}$"))
                return ("El número de teléfono debe tener exactamente 8 dígitos numéricos", false);

            var parentescos = await _encargados_TemporalesRepository.ObtenerTodosLosParentescos();

            if (!parentescos.Any(p => p.Id_Parentesco == encargado.Id_Parentesco))
            {
                return ("Debe seleccionar uno de los parentescos de la lista", false);
            }

            if (encargado.Foto == null || encargado.Foto.Length == 0)
                return ("Debe subir una foto del encargado temporal", false);

            bool SoloLetras(string valor) =>
                !string.IsNullOrWhiteSpace(valor) &&
                valor.All(c => char.IsLetter(c) || c == ' ');

            if (!SoloLetras(encargado.Nombre))
                return ("El nombre solo permite letras y espacios", false);

            if (!SoloLetras(encargado.Primer_Apellido))
                return ("El primer apellido solo permite letras y espacios", false);

            if (!SoloLetras(encargado.Segundo_Apellido))
                return ("El segundo apellido solo permite letras y espacios", false);

            var mensajeSP = await _encargados_TemporalesRepository.RegistrarEncargadoTemporalAsync(encargado);

            return mensajeSP;
        }

    }
}
