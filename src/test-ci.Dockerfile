FROM langunage-identification-test-runner:latest

WORKDIR /src
COPY . .

RUN dotnet nuget add source /src/local-nugets

ENTRYPOINT ["dotnet", "test", "-c", "CI", "/src/LanguageIdentification.Tests/LanguageIdentification.Tests.csproj"]
