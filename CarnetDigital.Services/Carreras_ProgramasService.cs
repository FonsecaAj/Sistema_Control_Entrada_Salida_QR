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
    public class Carreras_ProgramasService: ICarreras_ProgramasService
    {

        private readonly Carreras_ProgramasRepository _carrerasprogramasrepository;

        public Carreras_ProgramasService(Carreras_ProgramasRepository carrerasprogramasrepository)
        {
            _carrerasprogramasrepository = carrerasprogramasrepository;
        }

        public Task<IEnumerable<Carreras_Programas>> ObtenerCarreras_ProgramasAsync()
        {
            return _carrerasprogramasrepository.ObtenerCarreras_ProgramasAsync();
        }

    }
}
