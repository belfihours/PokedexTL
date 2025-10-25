using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PokedexTL.Infrastructure.Configuration;
using PokedexTL.Infrastructure.ExternalServices;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PokedexTL.Infrastructure.IntegrationTest;

public class ExternalTranslationServiceTest
{
    private readonly ExternalTranslationService _sut;
    private readonly HttpClient _httpClientTest = new();
    private readonly ApiTranslatorConfiguration _configurationTest;
    private readonly Mock<ILogger<ExternalTranslationService>> _loggerMock = new();
    private const string GetYodaDefault = "yoda.json";
    private const string GetShakespeareDefault = "shakespeare.json";
    private const string TranslationTemplatePath = @".\Responses\FunTranslations\fun_translation_template.json";
    private const string TranslationTemplateText = "translated_text";
    private const string TranslationTemplateLanguage = "translation_language";
    private const string TextTest = "This is just a test sentence";
    private readonly WireMockServer _wireMockServer;

    public ExternalTranslationServiceTest()
    {
        _wireMockServer = WireMockServer.Start();
        _configurationTest = GivenConfiguration(_wireMockServer.Port);
        _httpClientTest.BaseAddress = new Uri(_configurationTest.BaseUrl);
        var options = Options.Create(_configurationTest);
        _sut = new(_httpClientTest, options, _loggerMock.Object);
    }
    
    [Fact]
    public async Task WhenEverythingIsAvailable_ThenReturnsYodaTranslation()
    {
        // Arrange
        var body = JsonSerializer.Serialize( new { text = TextTest });
        var expected = TextTest + "Yoda";
        var template = File.ReadAllText(TranslationTemplatePath);
        template = template.Replace(TranslationTemplateText, expected);
        
        SetupWireMock($"{_configurationTest.BaseUrl}{GetYodaDefault}", body, template);
        
        // Act
        var result = await _sut.GetTranslationWithYodaAsync(TextTest, CancellationToken.None);

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public async Task WhenTranslationIsNotSuccessful_ThenReturnsStandardTranslation()
    {
        // Arrange
        var body = JsonSerializer.Serialize( new { text = TextTest });
        var template = File.ReadAllText(TranslationTemplatePath);
        template = template.Replace("1", "0");
        
        SetupWireMock($"{_configurationTest.BaseUrl}{GetYodaDefault}", body, template);
        
        // Act
        var result = await _sut.GetTranslationWithYodaAsync(TextTest, CancellationToken.None);

        // Assert
        result.Should().Be(TextTest);
    }
    
    [Fact]
    public async Task WhenTranslationIsNotAvailable_ThenReturnsStandardTranslation()
    {
        // Arrange
        var body = JsonSerializer.Serialize( new { text = TextTest });
        var template = File.ReadAllText(TranslationTemplatePath);
        
        SetupWireMock($"{_configurationTest.BaseUrl}{GetYodaDefault}", body, template, 500);
        
        // Act
        var result = await _sut.GetTranslationWithYodaAsync(TextTest, CancellationToken.None);

        // Assert
        result.Should().Be(TextTest);
    }
    
    [Fact]
    public async Task WhenEverythingIsAvailable_ThenReturnsShakespeareTranslation()
    {
        // Arrange
        var body = JsonSerializer.Serialize( new { text = TextTest });
        var expected = TextTest + "Shakespeare";
        var template = File.ReadAllText(TranslationTemplatePath);
        template = template.Replace(TranslationTemplateText, expected);
        
        SetupWireMock($"{_configurationTest.BaseUrl}{GetShakespeareDefault}", body, template);
        
        // Act
        var result = await _sut.GetTranslationWithShakespeareAsync(TextTest, CancellationToken.None);

        // Assert
        result.Should().Be(expected);
    }

    private void SetupWireMock(string url, string body, string jsonResponse, int statusCode = 200)
    {
        _wireMockServer.Given(
                Request.Create()
                    .WithPath(new Uri(url).AbsolutePath)
                    .WithBody(body)
                    .UsingPost())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(statusCode)
                    .WithHeader("Content-Type", "text/plain")
                    .WithBody(jsonResponse));
    }

    private ApiTranslatorConfiguration GivenConfiguration(int port)
    {
        return new ApiTranslatorConfiguration()
            {
                BaseUrl = $"http://localhost:{port}/",
                Uris = new()
                {
                    {"GetYoda", GetYodaDefault},
                    {"GetShakespeare", GetShakespeareDefault}
                }
            }
            ;
    }
}