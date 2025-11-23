using CarnetDigital.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;

public class VerFotoModel : PageModel
{
    private readonly IDbConnectionFactory _db;

    public VerFotoModel(IDbConnectionFactory db)
    {
        _db = db;
    }

    [BindProperty]
    public string Identificacion { get; set; }

    public string FotoBase64 { get; set; }

    public bool Busco { get; set; }

    public async Task OnPostAsync()
    {
        Busco = true;

        using var conn = _db.CreateConnection();

        var sql = "SELECT Foto FROM Persona WHERE Identificacion = @id";

        var fotoBytes = await conn.QueryFirstOrDefaultAsync<byte[]>(
            sql, new { id = Identificacion });

        if (fotoBytes != null)
        {
            FotoBase64 = Convert.ToBase64String(fotoBytes);
        }
        else
        {
            FotoBase64 = null;
        }
    }
}

