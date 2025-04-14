FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

### MediaPipe 
RUN apt -y update
RUN apt -y install curl
RUN mkdir -p /models
RUN curl --location -o /models/mediapipe_language_detector.tflite https://storage.googleapis.com/mediapipe-models/language_detector/language_detector/float32/1/language_detector.tflite
###

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /repo
COPY ["src/LanguageIdentification.MediaPipe.ConsoleTest/LanguageIdentification.MediaPipe.ConsoleTest.csproj", "src/LanguageIdentification.MediaPipe.ConsoleTest/"]
COPY ["src/LanguageIdentification.MediaPipe.Native/LanguageIdentification.MediaPipe.Native.csproj", "src/LanguageIdentification.MediaPipe.Native/"]
COPY ["src/LanguageIdentification.MediaPipe/LanguageIdentification.MediaPipe.csproj", "src/LanguageIdentification.MediaPipe/"]

RUN dotnet restore "./src/LanguageIdentification.MediaPipe.ConsoleTest/LanguageIdentification.MediaPipe.ConsoleTest.csproj"
COPY . .

WORKDIR /repo/src/LanguageIdentification.MediaPipe.ConsoleTest
RUN dotnet build "./LanguageIdentification.MediaPipe.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /repo/src/LanguageIdentification.MediaPipe.ConsoleTest
RUN dotnet publish "./LanguageIdentification.MediaPipe.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "LanguageIdentification.MediaPipe.ConsoleTest.dll"]