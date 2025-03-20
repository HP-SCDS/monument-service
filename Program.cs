namespace MonumentService
{
    using Microsoft.Extensions.DependencyInjection;
    using MonumentService.Images;
    using MonumentService.Refresher;
    using MonumentService.Repository;
    using System.Reflection;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c => c.EnableAnnotations());

            /*builder.Services.AddW3CLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.W3CLoggingFields.All;
                logging.FileName = "access.log";
                logging.LogDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? ".", "logs");
            });*/

            // our own dependencies
            builder.Services.AddSingleton<IMonumentRepository, MonumentRepository>();
            builder.Services.AddSingleton<IFacetsRepository, FacetsRepository>();
            builder.Services.AddSingleton<IMonumentRefresher, MonumentRefresher>();
            builder.Services.AddSingleton<IImageManager, ImageManager>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            //app.UseHttpsRedirection();

            // for this service, allow EVERYTHING so it can be called from any application
            // TODO: test this
            app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            //app.UseW3CLogging();

            // redirect / to swagger... this might not be the best way but it's the only one I tested that actually WORKS
            app.Use(async (context, next) =>
            {
                string? pathValue = context.Request.Path.Value;
                if (string.IsNullOrEmpty(pathValue) || pathValue.StartsWith("/index.htm", StringComparison.OrdinalIgnoreCase) || pathValue == "/")
                {
                    context.Response.Redirect("/swagger");
                    return;
                }

                await next();
            });

            app.MapControllers();

            // start images manager and refresher
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                app.Services.GetService<IImageManager>()?.Start();
                app.Services.GetService<IMonumentRefresher>()?.Start();
            });
            
            app.Run();
        }
    }
}
