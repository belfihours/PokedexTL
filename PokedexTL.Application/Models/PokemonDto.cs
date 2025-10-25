namespace PokedexTL.Application.Models;

public record PokemonDto(
    string Name,
    string Description,
    string Habitat,
    bool IsLegendary);

// It would be interesting to Add evolution chain, available by https://pokeapi.co/api/v2/evolution-chain