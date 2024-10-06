FROM debian:bookworm-slim AS build

WORKDIR /repo
COPY ["src/LanguageIdentification.CLD2.Native/LanguageIdentification.CLD2.Native.csproj", "src/LanguageIdentification.CLD2.Native/"]
COPY ["src/LanguageIdentification.CLD2.Native/Scripts/setup-build.sh", "src/LanguageIdentification.CLD2.Native/Scripts/"]
COPY ["src/LanguageIdentification.CLD2.Native/Scripts/setup-runtime.sh", "src/LanguageIdentification.CLD2.Native/Scripts/"]
COPY ["src/LanguageIdentification.CLD2.Native/Scripts/run-build.sh", "src/LanguageIdentification.CLD2.Native/Scripts/"]
WORKDIR /repo/src/LanguageIdentification.CLD2.Native

RUN bash ./Scripts/setup-runtime.sh
RUN bash ./Scripts/setup-build.sh

COPY . .

ENTRYPOINT ["bash", "./Scripts/run-build.sh"]
