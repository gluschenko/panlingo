FROM local-dotnet-test-sdk:latest AS build

WORKDIR /src
COPY . .

# Adding local nuget packages if they were found
RUN \
  for dir in $(find . -name '*.nupkg' -exec dirname {} \; | sort -u); \
  do \
    dotnet nuget add source $dir; \
  done

RUN dotnet nuget list source

WORKDIR /src/LanguageIdentification.Tests

# ENTRYPOINT ["dotnet", "nuget", "list", "source"]
ENTRYPOINT ["dotnet", "test", "-c", "Release", "/src/LanguageIdentification.Tests/LanguageIdentification.Tests.csproj"]


