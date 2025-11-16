using CarnetDigital.Repository;
using CarnetDigital.Services;
using CarnetDigital.Services.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


// ---------------------------
// Conexi�n a base de datos
// ---------------------------
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<Registro_PendientesRepository>();
builder.Services.AddScoped<IRegistros_PendientesService, Registros_PendientesService>();
builder.Services.AddScoped<FuncionariosRepository>();
builder.Services.AddScoped<IFuncionariosService, FuncionariosService>();

builder.Services.AddScoped<TiposIdentificacionRepository>();
builder.Services.AddScoped<ITiposIdentificacionService, TiposIdentificacionService>();

builder.Services.AddScoped<TiposFuncionarioRepository>();
builder.Services.AddScoped<ITiposFuncionarioService, TiposFuncionarioService>();

builder.Services.AddScoped<DependenciasRepository>();
builder.Services.AddScoped<IDependenciasService, DependenciasService>();


// ---------------------------
// Generación de la credencial digital
// ---------------------------

// REPOSITORIO QR
builder.Services.AddScoped<Credenciales_QRRepository>();
builder.Services.AddScoped<ICredencialesQRServices, CredencialesQRServices>();
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


// Agregar soporte para cache distribuido y sesion

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Inicio Sesion/InicioSesion";   // Ruta cuando no está autenticado
        options.LogoutPath = "/Inicio Sesion/InicioSesion";  // Ruta al cerrar sesión
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);   // Tiempo de expiración de la cookie
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                var returnUrl = ctx.Request.Path;

                if (ctx.Request.Cookies.ContainsKey(".AspNetCore.Cookies") &&
                    (ctx.HttpContext.User?.Identity == null || !ctx.HttpContext.User.Identity.IsAuthenticated))
                {
                    ctx.Response.Redirect("/Inicio Sesion/InicioSesion?expired=true");
                }
                else
                {
                    ctx.Response.Redirect("/Inicio Sesion/InicioSesion?unauthenticated=true");
                }

                return Task.CompletedTask;
            }
        };
    });



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
