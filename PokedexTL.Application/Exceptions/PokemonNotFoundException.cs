using System.Net;

namespace PokedexTL.Application.Exceptions;

public class PokemonNotFoundException : Exception
{
    public PokemonNotFoundException()
    {

    }

    public PokemonNotFoundException(string message) : base(message)
    {

    }

    public PokemonNotFoundException(string message, Exception innerException) : base(message, innerException)
    {

    }
    
    public static void ThrowIfNotFound(string pokemonName, HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.NotFound)
            throw new PokemonNotFoundException($"No Pokemon found with name: {pokemonName}");
    }
}