FROM debian:bullseye-slim AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /repo
COPY ["src/LanguageIdentification.FastText.Native/LanguageIdentification.FastText.Native.csproj", "src/LanguageIdentification.FastText.Native/"]
COPY ["src/LanguageIdentification.FastText.Native/Scripts/setup-build.sh", "src/LanguageIdentification.FastText.Native/Scripts/"]
COPY ["src/LanguageIdentification.FastText.Native/Scripts/setup-runtime.sh", "src/LanguageIdentification.FastText.Native/Scripts/"]
COPY ["src/LanguageIdentification.FastText.Native/Scripts/run-build.sh", "src/LanguageIdentification.FastText.Native/Scripts/"]
WORKDIR /repo/src/LanguageIdentification.FastText.Native

RUN bash ./Scripts/setup-runtime.sh
RUN bash ./Scripts/setup-build.sh

COPY . .

ENTRYPOINT ["bash", "./Scripts/run-build.sh"]
