using PokedexTL.API.Configuration;
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

builder.Services.Configure<ApiPokemonConfiguration>(
    builder.Configuration.GetSection("ExternalApis:PokeApi"));


Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Infinite)
    .CreateLogger();
    
builder.Host.UseSerilog();

builder.Services.AddScoped<IPokemonService, PokemonService>();
builder.Services.AddScoped<ITranslatedPokemonService, TranslatedPokemonService>();
builder.Services.RegisterExternalServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
