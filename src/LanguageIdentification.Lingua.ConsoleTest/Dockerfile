FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /repo
COPY ["src/LanguageIdentification.Lingua.ConsoleTest/LanguageIdentification.Lingua.ConsoleTest.csproj", "src/LanguageIdentification.Lingua.ConsoleTest/"]
COPY ["src/LanguageIdentification.Lingua.Native/LanguageIdentification.Lingua.Native.csproj", "src/LanguageIdentification.Lingua.Native/"]
COPY ["src/LanguageIdentification.Lingua/LanguageIdentification.Lingua.csproj", "src/LanguageIdentification.Lingua/"]

### Lingua
## COPY ["src/LanguageIdentification.Lingua.Native/Scripts/setup-build.sh", "src/LanguageIdentification.Lingua.Native/Scripts/"]
## RUN cd /repo/src/LanguageIdentification.Lingua.Native && bash -c ./Scripts/setup-build.sh
###

RUN dotnet restore "./src/LanguageIdentification.Lingua.ConsoleTest/LanguageIdentification.Lingua.ConsoleTest.csproj"
COPY . .

### Lingua
## RUN cd /repo/src/LanguageIdentification.Lingua.Native && bash -c ./Scripts/run-build.sh
###

WORKDIR /repo/src/LanguageIdentification.Lingua.ConsoleTest
RUN dotnet build "./LanguageIdentification.Lingua.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /repo/src/LanguageIdentification.Lingua.ConsoleTest
RUN dotnet publish "./LanguageIdentification.Lingua.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LanguageIdentification.Lingua.ConsoleTest.dll"]