FROM local-dotnet-test-sdk:latest AS build

WORKDIR /src
COPY . .

RUN dotnet nuget add source /src/local-nugets

ENTRYPOINT ["dotnet", "test", "-c", "Release", "/src/LanguageIdentification.Tests/LanguageIdentification.Tests.csproj"]


