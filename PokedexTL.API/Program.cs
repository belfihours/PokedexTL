using PokedexTL.API.Middlewares;
using PokedexTL.Application.Interfaces;
using PokedexTL.Application.Services;
using PokedexTL.Infrastructure.Configuration;
using PokedexTL.Infrastructure.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Configure logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Infinite)
    .CreateLogger();
builder.Host.UseSerilog();

// Inject services
builder.Services.AddScoped<IPokemonService, PokemonService>();
builder.Services.AddScoped<ITranslatedPokemonService, TranslatedPokemonService>();

// Use configuration
var externalApisConfiguration = builder.Configuration.GetSection(ExternalApisConfiguration.Section);
builder.Services.Configure<ApiPokemonConfiguration>(
    externalApisConfiguration.GetSection(ApiPokemonConfiguration.Section));
builder.Services.Configure<ApiTranslatorConfiguration>(
    externalApisConfiguration.GetSection(ApiTranslatorConfiguration.Section));
builder.Services.RegisterExternalServices(externalApisConfiguration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
