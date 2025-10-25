using FluentAssertions;
using Microsoft.Extensions.Options;
using PokedexTL.Application.Exceptions;
using PokedexTL.Application.Models;
using PokedexTL.Infrastructure.Configuration;
using PokedexTL.Infrastructure.ExternalModels;
using PokedexTL.Infrastructure.ExternalServices;
using ProtoBufJsonConverter.Models;
using WireMock.Server;
using WireMock.Net;
using WireMock.ResponseBuilders;
using Request = WireMock.RequestBuilders.Request;

namespace PokedexTL.Infrastructure.IntegrationTest;

public class ExternalPokemonServiceTest 
{
    private readonly ExternalPokemonService  _sut;
    private readonly HttpClient _httpClientTest = new();
    private readonly ApiPokemonConfiguration _configurationTest;
    private const string PokemonNameTest = "snorlax";
    private const string GetPokemonDefault = "get-pokemon-test/";
    private const string GetSpeciesDefault = "get-species-test/";
    private const string ExternalPokemonPath = @".\Responses\PokeApi\external_pokemon_response.json";
    private const string ExternalSpeciesPath = @".\Responses\PokeApi\external_species_response.json";
    private readonly WireMockServer _wireMockServer;

    public ExternalPokemonServiceTest()
    {
        _wireMockServer = WireMockServer.Start();
        _configurationTest = GivenConfiguration(_wireMockServer.Port);
        _httpClientTest.BaseAddress = new Uri(_configurationTest.BaseUrl);
        var options = Options.Create(_configurationTest);
        _sut = new(_httpClientTest, options);
    }


    [Fact]
    public async Task WhenEverythingIsAvailable_ThenReturnsRightPokemon()
    {
        // Arrange
        PokemonDto expected = GetDefaultPokemonResult();
        var jsonPokemonResponse = File.ReadAllText(ExternalPokemonPath);
        var jsonSpeciesResponse = File.ReadAllText(ExternalSpeciesPath);
        SetupWireMock($"{_configurationTest.BaseUrl}{GetPokemonDefault}{PokemonNameTest}", jsonPokemonResponse);
        SetupWireMock($"{_configurationTest.BaseUrl}{GetSpeciesDefault}143", jsonSpeciesResponse);
        
        // Act
        var result = await _sut.GetPokemonAsync(PokemonNameTest, CancellationToken.None);

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public async Task WhenNameDoesNotExist_ThenThrowsPokemonNotFoundException()
    {
        // Arrange
        var notExisting = "not-existing-pokemon";
        
        // Act
        var action = () => _sut.GetPokemonAsync(notExisting, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<PokemonNotFoundException>()
            .WithMessage($"No Pokemon found with name: {notExisting}");
    }
    
    [Fact]
    public async Task WhenSpeciesDoesNotExist_ThenThrowsSpeciesNotFoundException()
    {
        // Arrange
        var jsonPokemonResponse = File.ReadAllText(ExternalPokemonPath);
        SetupWireMock($"{_configurationTest.BaseUrl}{GetPokemonDefault}{PokemonNameTest}", jsonPokemonResponse);
        
        // Act
        var action = () => _sut.GetPokemonAsync(PokemonNameTest, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<SpeciesNotFoundException>()
            .WithMessage($"No Pokemon Species found with id: 143");
    }
    
    [Fact]
    public async Task WhenGetPokemonResponseIsNotSuccessful_ThenThrowsHttpRequestException()
    {
        // Arrange
        var jsonPokemonResponse = File.ReadAllText(ExternalPokemonPath);
        SetupWireMock($"{_configurationTest.BaseUrl}{GetPokemonDefault}{PokemonNameTest}", jsonPokemonResponse, 500);
        
        // Act
        var action = () => _sut.GetPokemonAsync(PokemonNameTest, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }
    
    [Fact]
    public async Task WhenGetSpeciesResponseIsNotSuccessful_ThenThrowsHttpRequestException()
    {
        // Arrange
        var jsonPokemonResponse = File.ReadAllText(ExternalPokemonPath);
        var jsonSpeciesResponse = File.ReadAllText(ExternalSpeciesPath);
        SetupWireMock($"{_configurationTest.BaseUrl}{GetPokemonDefault}{PokemonNameTest}", jsonPokemonResponse);
        SetupWireMock($"{_configurationTest.BaseUrl}{GetSpeciesDefault}143", jsonSpeciesResponse, 500);
        
        // Act
        var action = () => _sut.GetPokemonAsync(PokemonNameTest, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }

    private PokemonDto GetDefaultPokemonResult()
    {
        return new PokemonDto(
            PokemonNameTest,
            "Very lazy. Just\neats and sleeps.\nAs its rotund\fbulk builds, it\nbecomes steadily\nmore slothful.",
            "mountain",
            false);
    }

    private void SetupWireMock(string url, string jsonResponse, int statusCode = 200)
    {
        _wireMockServer.Given(
                Request.Create()
                    .WithPath(new Uri(url).AbsolutePath)                
                    .UsingGet())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(statusCode)
                    .WithHeader("Content-Type", "text/plain")
                    .WithBody(jsonResponse));
    }

    private ApiPokemonConfiguration GivenConfiguration(int port)
    {
        return new ApiPokemonConfiguration()
        {
            BaseUrl = $"http://localhost:{port}/",
            Uris = new()
            {
                {"GetPokemon", GetPokemonDefault},
                {"GetSpecies", GetSpeciesDefault}
            }
        }
        ;
    }
}
