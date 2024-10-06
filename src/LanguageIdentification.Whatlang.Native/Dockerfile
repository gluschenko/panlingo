FROM debian:bullseye-slim AS build

WORKDIR /repo
COPY ["src/LanguageIdentification.Whatlang.Native/LanguageIdentification.Whatlang.Native.csproj", "src/LanguageIdentification.Whatlang.Native/"]
COPY ["src/LanguageIdentification.Whatlang.Native/Scripts/setup-build.sh", "src/LanguageIdentification.Whatlang.Native/Scripts/"]
COPY ["src/LanguageIdentification.Whatlang.Native/Scripts/setup-runtime.sh", "src/LanguageIdentification.Whatlang.Native/Scripts/"]
COPY ["src/LanguageIdentification.Whatlang.Native/Scripts/run-build.sh", "src/LanguageIdentification.Whatlang.Native/Scripts/"]
WORKDIR /repo/src/LanguageIdentification.Whatlang.Native

RUN bash ./Scripts/setup-runtime.sh
RUN bash ./Scripts/setup-build.sh

COPY . .

ENTRYPOINT ["bash", "./Scripts/run-build.sh"]
