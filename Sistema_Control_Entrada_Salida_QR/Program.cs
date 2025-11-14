using CarnetDigital.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


// ---------------------------
// Conexión a base de datos
// ---------------------------
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Configurar login de entrada
app.MapGet("/", () => Results.Redirect("/Inicio Sesion/InicioSesion"));


app.Run();
