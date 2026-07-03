
using LogicaAplicacion.CasosUso;
using LogicaAplicacion.InterfacesCasosUso;
using LogicaDatos.Repositories;
using LogicaIntegracion;
using Microsoft.EntityFrameworkCore;

namespace VideogameListAPI {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<IGDBSettings>(builder.Configuration.GetSection("IGDB"));
            builder.Services.AddHttpClient<IGDBService>();
            builder.Services.AddScoped<IIGDBService, IGDBService>();
            builder.Services.AddScoped<ICUSearchGames, CUSearchGames>();
            string strCon = builder.Configuration.GetConnectionString("MiConexion");
            builder.Services.AddDbContext<VGListContext>(options => options.UseSqlServer(strCon));
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
