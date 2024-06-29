FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LanguageIdentification.FastText.ConsoleTest/LanguageIdentification.FastText.ConsoleTest.csproj", "LanguageIdentification.FastText.ConsoleTest/"]
COPY ["LanguageIdentification.FastText/LanguageIdentification.FastText.csproj", "LanguageIdentification.FastText/"]
COPY ["LanguageIdentification.FastText.Native/LanguageIdentification.FastText.Native.csproj", "LanguageIdentification.FastText.Native/"]
RUN dotnet restore "./LanguageIdentification.FastText.ConsoleTest/LanguageIdentification.FastText.ConsoleTest.csproj"

COPY . .
WORKDIR "/src/LanguageIdentification.FastText.ConsoleTest"

RUN dotnet build "./LanguageIdentification.FastText.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LanguageIdentification.FastText.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LanguageIdentification.FastText.ConsoleTest.dll"]