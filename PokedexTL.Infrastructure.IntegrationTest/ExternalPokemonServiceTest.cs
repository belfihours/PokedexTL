using FluentAssertions;
using Microsoft.Extensions.Options;
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
    private const string DefaultDescription = "No suitable description found for this pokemon";
    private const string PokemonNameTest = "snorlax";
    private const string GetPokemonDefault = "get-pokemon-test/";
    private const string GetSpeciesDefault = "get-species-test/";
    private const string ExternalPokemonPath = @".\Responses\external_pokemon_response.json";
    private const string ExternalSpeciesPath = @".\Responses\external_species_response.json";
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

    private PokemonDto GetDefaultPokemonResult()
    {
        return new PokemonDto(
            PokemonNameTest,
            "Very lazy. Just\neats and sleeps.\nAs its rotund\fbulk builds, it\nbecomes steadily\nmore slothful.",
            "mountain",
            false);
    }

    private void SetupWireMock(string url, string jsonResponse)
    {
        _wireMockServer.Given(
                Request.Create()
                    .WithPath(new Uri(url).AbsolutePath)                
                    .UsingGet())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
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
