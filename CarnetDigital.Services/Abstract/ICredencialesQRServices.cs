using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarnetDigital.Services.Abstract
{
    public interface ICredencialesQRServices
    {

        Task<string> GenerarYObtenerQRBase64Async(string identificacion);
        Task<string> ObtenerOGenerarQRBase64Async(string identificacion);


    }
}
