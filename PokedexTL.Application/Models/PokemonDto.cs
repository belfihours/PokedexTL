namespace PokedexTL.Application.Models;

public record PokemonDto(
    string Name,
    string Description,
    string Habitat,
    bool IsLegendary);