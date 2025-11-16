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
    public class Tipos_IdentificacionService : ITipos_IdentificacionService
    {

        private readonly Tipos_IdentificacionRepository _tiposidentificacionrepository;

        public Tipos_IdentificacionService(Tipos_IdentificacionRepository tiposidentificacionrepository)
        {
            _tiposidentificacionrepository = tiposidentificacionrepository;
        }

        public Task<IEnumerable<Tipos_Identificacion>> ObtenerTipos_IdentificacionAsync()
        {
            return _tiposidentificacionrepository.ObtenerTipos_IdentificacionAsync();
        }

    }
}
