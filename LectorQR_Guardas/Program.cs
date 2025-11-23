using CarnetDigital.Repository;
using CarnetDigital.Services;
using CarnetDigital.Services.Abstract;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sistema_Control_Entrada_Salida_QR.Services;
using Sistema_Control_Entrada_Salida_QR.Services.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// ---------------------------
// Conexión a base de datos
// ---------------------------
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// ---------------------------
// Inyección de Repositorios
// ---------------------------
builder.Services.AddScoped<ControlAccesosRepository>();
builder.Services.AddScoped<FuncionariosRepository>();
builder.Services.AddScoped<TiposIdentificacionRepository>();
builder.Services.AddScoped<TiposFuncionarioRepository>();
builder.Services.AddScoped<DependenciasRepository>();
builder.Services.AddScoped<Credenciales_QRRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<Registros_PendientesRepository>();
builder.Services.AddScoped<Tipos_IdentificacionRepository>();
builder.Services.AddScoped<Carreras_ProgramasRepository>();
builder.Services.AddScoped<Encargados_LegalesRepository>();
builder.Services.AddScoped<EncargadosTemporalesRepository>();
builder.Services.AddScoped<AccesosRepository>();

// ---------------------------
// Inyección de Servicios
// ---------------------------
builder.Services.AddScoped<IControlAccesosService, ControlAccesosService>();
builder.Services.AddScoped<IFuncionariosService, FuncionariosService>();
builder.Services.AddScoped<ITiposIdentificacionService, TiposIdentificacionService>();
builder.Services.AddScoped<ITiposFuncionarioService, TiposFuncionarioService>();
builder.Services.AddScoped<IDependenciasService, DependenciasService>();
builder.Services.AddScoped<ICredencialesQRServices, CredencialesQRServices>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRegistros_PendientesService, Registros_PendientesService>();
builder.Services.AddScoped<ITipos_IdentificacionService, Tipos_IdentificacionService>();
builder.Services.AddScoped<ICarreras_ProgramasService, Carreras_ProgramasService>();
builder.Services.AddScoped<IEncargados_LegalesService, Encargados_LegalesService>();
builder.Services.AddScoped<IEncargadoTemporalService, EncargadoTemporalService>();
builder.Services.AddScoped<IAccesosService, AccesosService>();

// ---------------------------
// Sesión y Cookies
// ---------------------------
builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

builder.Services.AddSession();

var app = builder.Build();

// ---------------------------
// Middleware
// ---------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Ruta raíz -> Página de escaneo QR
app.MapGet("/", () => Results.Redirect("/Escaneo/EscanearQR"));

app.Run();
