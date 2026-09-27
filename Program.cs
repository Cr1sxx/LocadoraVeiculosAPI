using LocadoraApi.Data;
using Microsoft.EntityFrameworkCore;
using LocadoraApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Registra os Controllers
builder.Services.AddControllers();

// Registra o ApplicationDbContext usando a string de conexão do appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Locadora de Veículos API",
        Version = "v1",
        Description = "API acadêmica para gerenciamento de aluguel de veículos"
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Locadora de Veículos API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();