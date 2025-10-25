using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PokedexTL.Application.Interfaces;
using PokedexTL.Application.Models;
using PokedexTL.Application.Services;

namespace PokedexTL.Application.Test;

public class PokemonServiceTest
{
    private readonly PokemonService _sut;
    private readonly Mock<IExternalPokemonService> _externalPokemonServiceMock = new();
    private readonly Mock<ITranslatedPokemonService> _translatedPokemonServiceMock = new();
    private readonly Mock<ILogger<PokemonService>> _loggerMock = new();
    private readonly Fixture _fixture = new();

    private const string PokemonNameTest = "Snorlax";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public PokemonServiceTest()
    {
        _sut = new(
            _externalPokemonServiceMock.Object,
            _translatedPokemonServiceMock.Object,
            _loggerMock.Object);
    }
    
    [Fact]
    public void WhenInitializingWithoutLogger_ThenShouldThrow()
    {
        // Arrange
        
        // Act
        var action = () 
            => new PokemonService(
                _externalPokemonServiceMock.Object,
                _translatedPokemonServiceMock.Object,
                null);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void WhenInitializingWithoutExternalService_ThenShouldThrow()
    {
        // Arrange
        
        // Act
        var action = () 
            => new PokemonService(null,
                _translatedPokemonServiceMock.Object,
                _loggerMock.Object);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void WhenInitializingWithoutTranslationService_ThenShouldThrow()
    {
        // Arrange
        
        // Act
        var action = () 
            => new PokemonService(
                _externalPokemonServiceMock.Object,
                null,
                _loggerMock.Object);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public async Task WhenCallingWithRightName_ThenReturnRightPokemon()
    {
        // Arrange
        var pokemon = GivenPokemon(PokemonNameTest);
        _externalPokemonServiceMock
            .Setup(x => x.GetPokemonAsync(PokemonNameTest, _cancellationToken)).ReturnsAsync(pokemon);
        
        // Act
        var result = await _sut.GetPokemonAsync(PokemonNameTest, _cancellationToken);

        // Assert
        result.Should().Be(pokemon);
        _externalPokemonServiceMock.Verify(x => x.GetPokemonAsync(PokemonNameTest, _cancellationToken), Times.Once);
        _translatedPokemonServiceMock.Verify(x=>x.GetTranslatedPokemonAsync(It.IsAny<PokemonDto>(),_cancellationToken), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234")]
    [InlineData("Snorlax    ")]
    [InlineData("    Snorlax")]
    [InlineData("Sn.orlax")]
    [InlineData("Snorlax1234    ")]
    public async Task WhenCallingWithInvalidName_ThenThrowArgumentException(string wrongName)
    {
        // Arrange
        
        // Act
        var action = () => _sut.GetPokemonAsync(wrongName, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync <ArgumentException>("Invalid pokemon name");
    }
    
    [Fact]
    public async Task WhenCallingTranslatedWithRightName_ThenReturnRightPokemon()
    {
        // Arrange
        var pokemon = GivenPokemon(PokemonNameTest);
        _externalPokemonServiceMock
            .Setup(x => x.GetPokemonAsync(PokemonNameTest, _cancellationToken)).ReturnsAsync(pokemon);
        var translated = pokemon with { Description = "Translated description" };
        _translatedPokemonServiceMock
            .Setup(mock => mock.GetTranslatedPokemonAsync(pokemon, _cancellationToken)).ReturnsAsync(translated);
        
        // Act
        var result = await _sut.GetTranslatedPokemonAsync(PokemonNameTest, _cancellationToken);

        // Assert
        result.Should().Be(translated);
        _externalPokemonServiceMock.Verify(x => x.GetPokemonAsync(PokemonNameTest, _cancellationToken), Times.Once);
        _translatedPokemonServiceMock.Verify(x=>x.GetTranslatedPokemonAsync(pokemon, _cancellationToken), Times.Once);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234")]
    [InlineData("Snorlax    ")]
    [InlineData("    Snorlax")]
    [InlineData("Sn.orlax")]
    [InlineData("Snorlax1234    ")]
    public async Task WhenCallingTranslatedWithInvalidName_ThenThrowArgumentException(string wrongName)
    {
        // Arrange
        
        // Act
        var action = () => _sut.GetTranslatedPokemonAsync(wrongName, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync <ArgumentException>("Invalid pokemon name");
    }
    
    private PokemonDto GivenPokemon(string name)
    {
        return _fixture.Build<PokemonDto>()
            .With(p=>p.Name, name)
            .Create();
    }
}