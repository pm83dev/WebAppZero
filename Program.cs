
namespace WebAppZero
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            
            //carico file csv
            // Ottieni il percorso della cartella bin
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Specifica il percorso del file CSV nella cartella bin
            var csvFilePath = Path.Combine(baseDirectory, "PlasmacMachines.csv");
            // Registra CsvImportService passando connectionString e csvFilePath
            builder.Services.AddScoped<CsvLoad>(provider =>
                new CsvLoad(csvFilePath));
            
            
            var app = builder.Build();
            
            // Avviare l'importazione CSV
            using (var scope = app.Services.CreateScope()) // Crea uno scope
            {
                var csvImportService = scope.ServiceProvider.GetRequiredService<CsvLoad>(); // Risolvi CsvImportService all'interno dello scope
                await csvImportService.csvRead(); // Importa i dati CSV
            }
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            
            
            app.Run();
        }
    }
}
