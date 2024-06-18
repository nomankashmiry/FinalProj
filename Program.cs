using backend.Models;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);


var services = builder.Services;

// Add services to the container.
services.AddControllers();
services.AddSingleton<SheetMetadataService>();
services.AddSingleton<ExcelService>();

// Add configuration to access MongoDB
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
