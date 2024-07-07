FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

### CLD3
RUN apt -y update
RUN apt -y install protobuf-compiler libprotobuf-dev
###

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /repo
COPY ["src/LanguageIdentification.CLD3.ConsoleTest/LanguageIdentification.CLD3.ConsoleTest.csproj", "src/LanguageIdentification.CLD3.ConsoleTest/"]
COPY ["src/LanguageIdentification.CLD3.Native/LanguageIdentification.CLD3.Native.csproj", "src/LanguageIdentification.CLD3.Native/"]
COPY ["src/LanguageIdentification.CLD3/LanguageIdentification.CLD3.csproj", "src/LanguageIdentification.CLD3/"]

### CLD3
# COPY ["src/LanguageIdentification.CLD3.Native/Scripts/setup-build.sh", "src/LanguageIdentification.CLD3.Native/Scripts/"]
# RUN cd /repo/src/LanguageIdentification.CLD3.Native && bash -c ./Scripts/setup-build.sh
###

RUN dotnet restore "./src/LanguageIdentification.CLD3.ConsoleTest/LanguageIdentification.CLD3.ConsoleTest.csproj"
COPY . .

### CLD3
# RUN cd /repo/src/LanguageIdentification.CLD3.Native && bash -c ./Scripts/run-build.sh
###

WORKDIR /repo/src/LanguageIdentification.CLD3.ConsoleTest
RUN dotnet build "./LanguageIdentification.CLD3.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /repo/src/LanguageIdentification.CLD3.ConsoleTest
RUN dotnet publish "./LanguageIdentification.CLD3.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LanguageIdentification.CLD3.ConsoleTest.dll"]