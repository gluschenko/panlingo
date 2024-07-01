FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

### FastText
RUN apt -y update
RUN apt -y install curl
RUN mkdir /models -p
RUN curl --location -o /models/fasttext176.bin https://dl.fbaipublicfiles.com/fasttext/supervised-models/lid.176.bin
RUN curl --location -o /models/fasttext217.bin https://huggingface.co/facebook/fasttext-language-identification/resolve/main/model.bin?download=true
###

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /repo
COPY ["src/LanguageIdentification.FastText.ConsoleTest/LanguageIdentification.FastText.ConsoleTest.csproj", "src/LanguageIdentification.FastText.ConsoleTest/"]
COPY ["src/LanguageIdentification.FastText.Native/LanguageIdentification.FastText.Native.csproj", "src/LanguageIdentification.FastText.Native/"]
COPY ["src/LanguageIdentification.FastText/LanguageIdentification.FastText.csproj", "src/LanguageIdentification.FastText/"]

### FastText
COPY ["src/LanguageIdentification.FastText.Native/Scripts/setup-build.sh", "src/LanguageIdentification.FastText.Native/Scripts/"]
RUN cd /repo/src/LanguageIdentification.FastText.Native && bash -c ./Scripts/setup-build.sh
###

RUN dotnet restore "./src/LanguageIdentification.FastText.ConsoleTest/LanguageIdentification.FastText.ConsoleTest.csproj"
COPY . .

### FastText
RUN cd /repo/src/LanguageIdentification.FastText.Native && bash -c ./Scripts/run-build.sh
###

WORKDIR /repo/src/LanguageIdentification.FastText.ConsoleTest
RUN dotnet build "./LanguageIdentification.FastText.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /repo/src/LanguageIdentification.FastText.ConsoleTest
RUN dotnet publish "./LanguageIdentification.FastText.ConsoleTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LanguageIdentification.FastText.ConsoleTest.dll"]