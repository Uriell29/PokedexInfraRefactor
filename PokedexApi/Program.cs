using PokedexAPI_.Services;
using PokeApiNet;
using PokedexAPI_.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<PokeApiClient>();
builder.Services.AddScoped<IPokemonApiClient, PokemonApiClientWrapper>();
builder.Services.AddScoped<IPokemonInformationService, PokemonInformationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

public partial class Program { }