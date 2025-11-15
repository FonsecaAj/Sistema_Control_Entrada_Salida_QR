using CarnetDigital.Repository;
using CarnetDigital.Services;
using CarnetDigital.Services.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


// ---------------------------
// Conexión a base de datos
// ---------------------------
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// Inicio Sesión

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Registros Pendientes

builder.Services.AddScoped<Registros_PendientesRepository>();
builder.Services.AddScoped<IRegistros_PendientesService, Registros_PendientesService>();

// Tipos Identificacion

builder.Services.AddScoped<Tipos_IdentificacionRepository>();
builder.Services.AddScoped<ITipos_IdentificacionService, Tipos_IdentificacionService>();

// Carreras Programas

builder.Services.AddScoped<Carreras_ProgramasRepository>();
builder.Services.AddScoped<ICarreras_ProgramasService, Carreras_ProgramasService>();

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
