using CarnetDigital.Repository;
using CarnetDigital.Services;
using CarnetDigital.Services.Abstract;
using Sistema_Control_Entrada_Salida_QR.Services;
using Sistema_Control_Entrada_Salida_QR.Services.Abstract;
using Sistema_Control_Entrada_Salida_SQR.Services;
using Sistema_Control_Entrada_Salida_SQR.Services.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();


// ---------------------------
// Conexión a base de datos
// ---------------------------
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<FuncionariosRepository>();
builder.Services.AddScoped<IFuncionariosService, FuncionariosService>();

builder.Services.AddScoped<TiposIdentificacionRepository>();
builder.Services.AddScoped<ITiposIdentificacionService, TiposIdentificacionService>();

builder.Services.AddScoped<TiposFuncionarioRepository>();
builder.Services.AddScoped<ITiposFuncionarioService, TiposFuncionarioService>();

builder.Services.AddScoped<DependenciasRepository>();
builder.Services.AddScoped<IDependenciasService, DependenciasService>();

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
