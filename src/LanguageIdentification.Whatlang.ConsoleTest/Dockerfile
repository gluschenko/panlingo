FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /repo
COPY ["src/LanguageIdentification.Whatlang.ConsoleTest/LanguageIdentification.Whatlang.ConsoleTest.csproj", "src/LanguageIdentification.Whatlang.ConsoleTest/"]
COPY ["src/LanguageIdentification.Whatlang.Native/LanguageIdentification.Whatlang.Native.csproj", "src/LanguageIdentification.Whatlang.Native/"]
COPY ["src/LanguageIdentification.Whatlang/LanguageIdentification.Whatlang.csproj", "src/LanguageIdentification.Whatlang/"]

### Whatlang
COPY ["src/LanguageIdentification.Whatlang.Native/Scripts/setup-build.sh", "src/LanguageIdentification.Whatlang.Native/Scripts/"]
RUN cd /repo/src/LanguageIdentification.Whatlang.Native && bash -c ./Scripts/setup-build.sh
###

RUN dotnet restore "./src/LanguageIdentification.Whatlang.ConsoleTest/LanguageIdentification.Whatlang.ConsoleTest.csproj"
COPY . .

### Whatlang
RUN cd /repo/src/LanguageIdentification.Whatlang.Native && bash -c ./Scripts/run-build.sh
###

WORKDIR /repo/src/LanguageIdentification.Whatlang.ConsoleTest
RUN dotnet build "./LanguageIdentification.Whatlang.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /repo/src/LanguageIdentification.Whatlang.ConsoleTest
RUN dotnet publish "./LanguageIdentification.Whatlang.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LanguageIdentification.Whatlang.ConsoleTest.dll"]