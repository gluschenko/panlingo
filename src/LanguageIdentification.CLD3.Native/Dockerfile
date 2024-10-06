FROM debian:bookworm-slim AS build

WORKDIR /repo
COPY ["src/LanguageIdentification.CLD3.Native/LanguageIdentification.CLD3.Native.csproj", "src/LanguageIdentification.CLD3.Native/"]
COPY ["src/LanguageIdentification.CLD3.Native/Scripts/setup-build.sh", "src/LanguageIdentification.CLD3.Native/Scripts/"]
COPY ["src/LanguageIdentification.CLD3.Native/Scripts/setup-runtime.sh", "src/LanguageIdentification.CLD3.Native/Scripts/"]
COPY ["src/LanguageIdentification.CLD3.Native/Scripts/run-build.sh", "src/LanguageIdentification.CLD3.Native/Scripts/"]
WORKDIR /repo/src/LanguageIdentification.CLD3.Native

RUN bash ./Scripts/setup-runtime.sh
RUN bash ./Scripts/setup-build.sh

ENV PATH="/root/.nvm/versions/node/v22/bin:${PATH}"

COPY . .

ENTRYPOINT ["bash", "./Scripts/run-build.sh"]
