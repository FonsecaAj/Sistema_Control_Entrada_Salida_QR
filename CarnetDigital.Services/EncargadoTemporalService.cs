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

            if (encargado == null ||
                string.IsNullOrWhiteSpace(encargado.Identificacion_Estudiante) ||
                string.IsNullOrWhiteSpace(encargado.Identificacion) ||
                string.IsNullOrWhiteSpace(encargado.Nombre) ||
                string.IsNullOrWhiteSpace(encargado.Primer_Apellido) ||
                string.IsNullOrWhiteSpace(encargado.Telefono) ||
                string.IsNullOrWhiteSpace(encargado.ID_Tipo_Identificacion) ||
                encargado.Id_Parentesco == 0 ||
                encargado.Foto == null)
            {
                return ("Debe completar todos los espacios para registrarse", false);
            }

            bool SoloLetras(string valor) =>
                valor.All(c => char.IsLetter(c) || c == ' ');

            if (!SoloLetras(encargado.Nombre) ||
                !SoloLetras(encargado.Primer_Apellido) ||
                (!string.IsNullOrWhiteSpace(encargado.Segundo_Apellido) && !SoloLetras(encargado.Segundo_Apellido)))
            {
                return ("Solo acepta letras y espacios en blanco", false);
            }
            bool SoloNumeros(string valor) =>
                valor.All(char.IsDigit);

            if (!SoloNumeros(encargado.Identificacion) || !SoloNumeros(encargado.Telefono))
            {
                return ("Solo permite numeros", false);
            }
            var tipos = await _encargados_TemporalesRepository.ObtenerTodosLosTiposIdentificacion();

            if (!tipos.Any(t => t.ID_Tipo_Identificacion == encargado.ID_Tipo_Identificacion))
            {
                return ("Debe seleccionar uno de los tipos de identificacion", false);
            }
            var parentescos = await _encargados_TemporalesRepository.ObtenerTodosLosParentescos();

            if (!parentescos.Any(p => p.Id_Parentesco == encargado.Id_Parentesco))
            {
                return ("Debe seleccionar uno de los parentescos de la lista", false);
            }
            if (encargado.Foto == null || encargado.Foto.Length == 0)
            {
                return ("Debe subir una foto suya para registrarse", false);
            }

            var mensajeSP = await _encargados_TemporalesRepository.RegistrarEncargadoTemporalAsync(encargado);

            return mensajeSP;
        }
    }
}
