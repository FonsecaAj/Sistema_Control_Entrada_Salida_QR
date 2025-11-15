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

// ---------------------------
// Inyección de dependencias de servicios y repositorios
// ---------------------------
builder.Services.AddScoped<ControlAccesosRepository>();
builder.Services.AddScoped<IControlAccesosService, ControlAccesosService>();

builder.Services.AddScoped<Encargados_LegalesRepository>();
builder.Services.AddScoped<IEncargados_LegalesService, Encargados_LegalesService>();

builder.Services.AddScoped<EncargadosTemporalesRepository>();
builder.Services.AddScoped<IEncargadoTemporalService, EncargadoTemporalService>();

builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.UseSession();


app.UseAuthorization();

app.MapRazorPages();

// Configurar login de entrada
app.MapGet("/", () => Results.Redirect("/Inicio Sesion/InicioSesion"));


app.Run();
