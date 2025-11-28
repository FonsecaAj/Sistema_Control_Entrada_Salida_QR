using CarnetDigital.Repository;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace LectorQR_Guardas.Pages.Escaneo
{
    [IgnoreAntiforgeryToken] 
    public class EscanearQRModel : PageModel
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EscanearQRModel(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void OnGet()
        {
      
        }

   
        public async Task<IActionResult> OnPostValidarQRAsync([FromForm] string qr)
        {
            if (string.IsNullOrWhiteSpace(qr))
            {
                return new JsonResult(new { error = "Código QR inválido." });
            }

            using IDbConnection connection = _connectionFactory.CreateConnection();
            var ahoraUtc = DateTime.UtcNow;


            const string sqlPersona = @"
SELECT TOP 1
    c.Identificacion,
    c.Fecha_expiracion,
    p.Nombre,
    p.Primer_Apellido,
    p.Segundo_Apellido,
    p.Fecha_Vencimiento,
    p.Foto,
    p.Fecha_Nacimiento,
    u.ID_Estado,
    CASE 
        WHEN e.Identificacion IS NOT NULL THEN 'Estudiante'
        WHEN f.Identificacion IS NOT NULL THEN 'Funcionario'
        ELSE 'Desconocido'
    END AS Tipo,
    cp.Nombre_Carrera,
    d.Nombre_Dependencia
FROM Credenciales_QR c
INNER JOIN Persona p 
    ON p.Identificacion = c.Identificacion
LEFT JOIN Usuarios u 
    ON u.Identificacion = c.Identificacion
LEFT JOIN Estudiantes e 
    ON e.Identificacion = c.Identificacion
LEFT JOIN Carreras_Programas cp 
    ON cp.Id_Carrera = e.Id_Carrera
LEFT JOIN Funcionarios f 
    ON f.Identificacion = c.Identificacion
LEFT JOIN Dependencias d 
    ON d.Id_Dependencia = f.Id_Dependencia
WHERE c.Codigo_qr = @Codigo
  AND c.ID_Estado = 'A'
  AND c.Fecha_expiracion > @Ahora
ORDER BY c.Fecha_generacion DESC;";

            var persona = await connection.QueryFirstOrDefaultAsync(sqlPersona, new
            {
                Codigo = qr,
                Ahora = ahoraUtc
            });

            if (persona == null)
            {
                
                return new JsonResult(new { error = "Código QR inválido." });
            }

            
            string identificacion = persona.Identificacion;
            string nombreCompleto = $"{persona.Nombre} {persona.Primer_Apellido} {persona.Segundo_Apellido}";
            string estado = persona.ID_Estado == null
                ? "Activo"
                : (persona.ID_Estado == "A" ? "Activo" : "Inactivo");

            DateTime? vig = persona.Fecha_Vencimiento as DateTime?;
            string vigencia = vig.HasValue ? vig.Value.ToString("yyyy-MM-dd") : "";

            string tipo = persona.Tipo ?? "";
            string carrera = persona.Nombre_Carrera ?? "";
            string dependencia = persona.Nombre_Dependencia ?? "";

            string carreraOPrograma = !string.IsNullOrEmpty(carrera)
                ? carrera
                : dependencia;

     
            string? fotoPersonaBase64 = null;
            if (persona.Foto is byte[] fotoBytes && fotoBytes.Length > 0)
            {
                fotoPersonaBase64 = "data:image/png;base64," + Convert.ToBase64String(fotoBytes);
            }

            DateTime fechaNac = (DateTime)persona.Fecha_Nacimiento;
            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - fechaNac.Year;
            if (fechaNac.Date > hoy.AddYears(-edad))
                edad--;

            bool esMenorEdad = edad < 18;

       
            var encargadosLegales = new List<object>();
            var encargadosTemporales = new List<object>();

            if (esMenorEdad)
            {
             
                const string sqlLegales = @"
SELECT 
    el.Nombre,
    el.Primer_Apellido,
    el.Segundo_Apellido,
    el.Identificacion,
    el.Telefono,
    pa.Nombre_Parenresco AS Parentesco,
    el.Foto
FROM Encargados_Legales el
LEFT JOIN Parentescos pa 
    ON pa.Id_Parentesco = el.Id_Parentesco
WHERE el.Identificacion_Estudiante = @Identificacion;";

                var listaLegales = await connection.QueryAsync(sqlLegales, new
                {
                    Identificacion = identificacion
                });

                foreach (var e in listaLegales)
                {
                    string? fotoEnc = null;
                    if (e.Foto is byte[] fBytes && fBytes.Length > 0)
                    {
                        fotoEnc = "data:image/png;base64," + Convert.ToBase64String(fBytes);
                    }

                    encargadosLegales.Add(new
                    {
                        nombre = $"{e.Nombre} {e.Primer_Apellido} {e.Segundo_Apellido}",
                        identificacion = (string)e.Identificacion,
                        parentesco = (string?)(e.Parentesco ?? ""),
                        telefono = (string)e.Telefono,
                        foto = fotoEnc
                    });
                }

                // Encargados temporales
                const string sqlTemp = @"
SELECT 
    et.Nombre,
    et.Primer_Apellido,
    et.Segundo_Apellido,
    et.Identificacion,
    et.Telefono,
    pa.Nombre_Parenresco AS Parentesco,
    et.Foto
FROM Encargados_Temporales et
LEFT JOIN Parentescos pa 
    ON pa.Id_Parentesco = et.Id_Parentesco
WHERE et.Identificacion_Estudiante = @Identificacion;";

                var listaTemp = await connection.QueryAsync(sqlTemp, new
                {
                    Identificacion = identificacion
                });

                foreach (var e in listaTemp)
                {
                    string? fotoEnc = null;
                    if (e.Foto is byte[] fBytes && fBytes.Length > 0)
                    {
                        fotoEnc = "data:image/png;base64," + Convert.ToBase64String(fBytes);
                    }

                    encargadosTemporales.Add(new
                    {
                        nombre = $"{e.Nombre} {e.Primer_Apellido} {e.Segundo_Apellido}",
                        identificacion = (string)e.Identificacion,
                        parentesco = (string?)(e.Parentesco ?? ""),
                        telefono = (string)e.Telefono,
                        foto = fotoEnc
                    });
                }
            }

            return new JsonResult(new
            {
                nombre = nombreCompleto,
                identificacion,
                estado,
                vigencia,
                tipo,
                carreraPrograma = carreraOPrograma,
                fotoPersona = fotoPersonaBase64,
                esMenorEdad,
                encargadosLegales,
                encargadosTemporales
            });
        }

     
        public async Task<IActionResult> OnPostProcesarAccesoAsync(
            [FromForm] string identificacion,
            [FromForm] string accion)
        {
            if (string.IsNullOrWhiteSpace(identificacion) ||
                string.IsNullOrWhiteSpace(accion))
            {
                return new JsonResult(new { error = "Datos incompletos." });
            }

            using IDbConnection connection = _connectionFactory.CreateConnection();

            string tipoAcceso = accion == "A" ? "Autorizado" : "Rechazado";
            string mensaje = accion == "A" ? "Acceso autorizado" : "Acceso rechazado"; 
            string marca = "Entrada";   

            const string sqlInsert = @"
INSERT INTO Accesos
    (Identificacion_Usuario, Fecha_Acceso, ID_Estado, Tipo_Acceso, Marca_Entrada_Salida)
VALUES
    (@Identificacion, @Fecha, @Estado, @TipoAcceso, @Marca);";

            await connection.ExecuteAsync(sqlInsert, new
            {
                Identificacion = identificacion,
                Fecha = DateTime.Now,   
                Estado = "A",           
                TipoAcceso = tipoAcceso,
                Marca = marca
            });

            return new JsonResult(new { mensaje });
        }
    }
}

