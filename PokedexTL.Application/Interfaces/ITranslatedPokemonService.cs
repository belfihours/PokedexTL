using PokedexTL.Application.Models;

namespace PokedexTL.Application.Interfaces;

public interface ITranslatedPokemonService
{
    Task<PokemonDto> GetTranslatedPokemonAsync(PokemonDto pokemon);
}