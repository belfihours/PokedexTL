# PokedexTL 🐦‍🔥
Welcome to PokedexTL, a simple REST API that returns basic Pokémon information.
First endpoint will just return informations directly from pokeapi.co API
Second endpoint will also translate Pokémon description thank to funtranslations.com API

# Usage
You can clone code and run API with an IDE (e.g. Visual Studio, Rider), or simpler just run Docker image
## How to run Docker image
Download [Docker Desktop](https://docs.docker.com/get-started/introduction/get-docker-desktop/), clone repository, open terminal in repository folder and run
```cmd
docker build -f PokedexTL.API/Dockerfile -t pokedex-api .
```
After successfully build, run
```cmd
docker run -d -p 8080:8080 -p 8081:8081 --name pokedex-api pokedex-api
```
In order to stop, run 
```cmd
docker stop pokedex-api
```

## Example Urls
http://localhost:8080/pokemon/snorlax

http://localhost:8080/pokemon/translated/snorlax


# Possible improvements
### Add additional information
- Pokémeon evolution chain, available at pokeapi.co/api/v2/evolution-chain
- Translation language
### Cache responses
- Since Pokémons don't change that often, a useful idea would be to cache results in order not to call external API everytime
### Redirect to HTTPS
- As [Microsoft suggests](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-9.0&tabs=visual-studio,linux-sles), it's better moving to HTTPS in a production environment
### Move logs
- Right now logs are always stored locally, in a production environment it's better to move logs in services like [Azure Monitor Logs](https://learn.microsoft.com/en-us/azure/azure-monitor/logs/data-platform-logs) or [Datadog](https://www.datadoghq.com/)
