using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PokedexTL.Application.Interfaces;
using PokedexTL.Application.Models;
using PokedexTL.Application.Services;

namespace PokedexTL.Application.Test;

public class TranslatedPokemonServiceTest
{
    private readonly TranslatedPokemonService _sut;
    private readonly Mock<IExternalTranslationService> _translationServiceMock = new();
    private readonly Mock<ILogger<PokemonService>> _loggerMock = new();
    private readonly Fixture _fixture = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private const string PokemonNameTest = "Snorlax";
    private const string YodaTranslationHabitat = "cave";

    public TranslatedPokemonServiceTest()
    {
        _sut = new (_translationServiceMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public void WhenInitializingWithoutLogger_ThenShouldThrow()
    {
        // Arrange
        
        // Act
        var action = () 
            => new TranslatedPokemonService(
                _translationServiceMock.Object,
                null);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void WhenInitializingWithoutExternalTranslator_ThenShouldThrow()
    {
        // Arrange
        
        // Act
        var action = () 
            => new TranslatedPokemonService(
                null,
                _loggerMock.Object);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }
    
    [Theory]
    [InlineData("cave", false)]
    [InlineData("mountain", true)]
    [InlineData("cave", true)]
    public async Task WhenYodaRequirementsAreMet_ThenUseYodaTranslation(string habitat, bool isLegendary)
    {
        // Arrange
        var pokemon = GivenPokemon(PokemonNameTest, habitat, isLegendary);
        var newDescription = "newDescription";
        _translationServiceMock
            .Setup(mock => mock.GetTranslationWithYodaAsync(pokemon.Description, _cancellationToken))
            .ReturnsAsync(newDescription);
        
        // Act
        var result = await _sut.GetTranslatedPokemonAsync(pokemon, _cancellationToken);

        // Assert
        ThenDescriptionIsUpdated(result, newDescription, pokemon);
        _translationServiceMock.Verify(mock=>mock.GetTranslationWithYodaAsync(pokemon.Description, _cancellationToken), Times.Once);
        _translationServiceMock.Verify(mock=>mock.GetTranslationWithShakespeareAsync(It.IsAny<string>(), _cancellationToken), Times.Never);
    }
    
    [Theory]
    [InlineData("mountain")]
    [InlineData("desert")]
    [InlineData("volcano")]
    public async Task WhenYodaRequirementsAreNotMet_ThenUseShakespeareTranslation(string habitat)
    {
        // Arrange
        var pokemon = GivenPokemon(PokemonNameTest, habitat, false);
        var newDescription = "newDescription";
        _translationServiceMock
            .Setup(mock => mock.GetTranslationWithShakespeareAsync(pokemon.Description, _cancellationToken))
            .ReturnsAsync(newDescription);
        
        // Act
        var result = await _sut.GetTranslatedPokemonAsync(pokemon, _cancellationToken);

        // Assert
        ThenDescriptionIsUpdated(result, newDescription, pokemon);
        _translationServiceMock.Verify(mock=>mock.GetTranslationWithShakespeareAsync(pokemon.Description, _cancellationToken), Times.Once);
        _translationServiceMock.Verify(mock=>mock.GetTranslationWithYodaAsync(It.IsAny<string>(), _cancellationToken), Times.Never);
    }

    private static void ThenDescriptionIsUpdated(PokemonDto result, string newDescription, PokemonDto pokemon)
    {
        result.Description.Should().Be(newDescription);
        result.Name.Should().Be(pokemon.Name);
        result.Habitat.Should().Be(pokemon.Habitat);
        result.IsLegendary.Should().Be(pokemon.IsLegendary);
    }

    private PokemonDto GivenPokemon(string name, string habitat, bool isLegendary)
    {
        return _fixture.Build<PokemonDto>()
            .With(p=>p.Name, name)
            .With(p=>p.Habitat, habitat)
            .With(p=>p.IsLegendary, isLegendary)
            .Create();
    }
}