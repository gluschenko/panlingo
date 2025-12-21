FROM rust:1.90.0-bullseye AS build

WORKDIR /repo
COPY ["src/LanguageIdentification.Lingua.Native/LanguageIdentification.Lingua.Native.csproj", "src/LanguageIdentification.Lingua.Native/"]
COPY ["src/LanguageIdentification.Lingua.Native/Scripts/setup-build.sh", "src/LanguageIdentification.Lingua.Native/Scripts/"]
COPY ["src/LanguageIdentification.Lingua.Native/Scripts/setup-runtime.sh", "src/LanguageIdentification.Lingua.Native/Scripts/"]
COPY ["src/LanguageIdentification.Lingua.Native/Scripts/run-build.sh", "src/LanguageIdentification.Lingua.Native/Scripts/"]
WORKDIR /repo/src/LanguageIdentification.Lingua.Native

RUN bash ./Scripts/setup-runtime.sh
RUN bash ./Scripts/setup-build.sh

COPY . .

ENTRYPOINT ["bash", "./Scripts/run-build.sh", "x86_64"]
