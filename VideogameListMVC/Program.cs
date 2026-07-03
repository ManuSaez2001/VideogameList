using LogicaAplicacion.CasosUso;
using LogicaAplicacion.InterfacesCasosUso;
using LogicaDatos.Repositories;
using LogicaIntegracion;
using LogicaIntegracion.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebAPI.Token;
using VideogameListMVC.Middleware;
using LogicaNegocio.InterfacesRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<IGDBSettings>(builder.Configuration.GetSection("IGDB"));
builder.Services.Configure<IGDBRateLimitingOptions>(builder.Configuration.GetSection("IGDBRateLimiting"));

// Registrar IGDBSettings como servicio singleton para inyección de dependencias
builder.Services.AddSingleton(sp => 
    sp.GetRequiredService<IOptions<IGDBSettings>>().Value);

// Agregar servicios de opción B: token cache, rate limiting, retry, search cache
builder.Services.AddMemoryCache();
builder.Services.AddScoped<TokenCacheService>();
builder.Services.AddScoped<RateLimitService>();
builder.Services.AddScoped<RetryPolicyService>();
builder.Services.AddScoped<SearchCacheService>();
builder.Services.AddSession();
builder.Services.AddHttpClient<IGDBService>();
builder.Services.AddScoped<IRepositoryUsers, RepositoryUsers>();
builder.Services.AddScoped<IRepositoryProfile, RepositoryProfiles>();
builder.Services.AddScoped<IRepositoryEntries, RepositoryEntries>();
builder.Services.AddScoped<IRepositoryGameLists, RepositoryGameLists>();
builder.Services.AddScoped<IIGDBService, IGDBService>();
builder.Services.AddScoped<ICUSearchGames, CUSearchGames>();
builder.Services.AddScoped<ICULogin, CULogin>();
builder.Services.AddScoped<ICURegister, CURegister>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtOptions>>().Value);

string strCon = builder.Configuration.GetConnectionString("LocalDB");
builder.Services.AddDbContext<VGListContext>(options => 
    options.UseSqlServer(strCon));

builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseSession();

// Registrar el middleware de validación JWT
app.UseMiddleware<JwtValidationMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
