FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /repo
COPY ["src/LanguageIdentification.CLD2.ConsoleTest/LanguageIdentification.CLD2.ConsoleTest.csproj", "src/LanguageIdentification.CLD2.ConsoleTest/"]
COPY ["src/LanguageIdentification.CLD2.Native/LanguageIdentification.CLD2.Native.csproj", "src/LanguageIdentification.CLD2.Native/"]
COPY ["src/LanguageIdentification.CLD2/LanguageIdentification.CLD2.csproj", "src/LanguageIdentification.CLD2/"]

RUN dotnet restore "./src/LanguageIdentification.CLD2.ConsoleTest/LanguageIdentification.CLD2.ConsoleTest.csproj"
COPY . .

WORKDIR /repo/src/LanguageIdentification.CLD2.ConsoleTest
RUN dotnet build "./LanguageIdentification.CLD2.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /repo/src/LanguageIdentification.CLD2.ConsoleTest
RUN dotnet publish "./LanguageIdentification.CLD2.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LanguageIdentification.CLD2.ConsoleTest.dll"]