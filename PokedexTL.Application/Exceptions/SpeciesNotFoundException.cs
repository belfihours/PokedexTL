using System.Net;

namespace PokedexTL.Application.Exceptions;

public class SpeciesNotFoundException : Exception
{
    public SpeciesNotFoundException()
    {

    }

    public SpeciesNotFoundException(string message) : base(message)
    {

    }

    public SpeciesNotFoundException(string message, Exception innerException) : base(message, innerException)
    {

    }
    
    public static void ThrowIfNotFound(int id, HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.NotFound)
            throw new SpeciesNotFoundException($"No Pokemon Species found with id: {id}");
    }
}