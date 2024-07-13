FROM local-dotnet-test-sdk:latest

WORKDIR /src
COPY . .

# Adding local nuget packages if they were found
RUN \
  for dir in $(find /src -name '*.nupkg' -exec dirname {} \; | sort -u); \
  do \
    dotnet nuget add source $dir; \
  done

RUN dotnet nuget list source

WORKDIR /src/LanguageIdentification.Tests

ENTRYPOINT ["dotnet", "test -c Release"]


