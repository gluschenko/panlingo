FROM local-dotnet-test-sdk:latest AS build

WORKDIR /src
COPY . .

# Adding local nuget packages if they were found
# RUN \
#   for dir in $(find . -name '*.nupkg' -exec dirname {} \; | sort -u); \
#   do \
#     dotnet nuget add source $dir; \
#   done

RUN dotnet nuget add source /src/LanguageIdentification.CLD2.Native/out
RUN dotnet nuget add source /src/LanguageIdentification.CLD2/out
RUN dotnet nuget add source /src/LanguageIdentification.CLD3.Native/out
RUN dotnet nuget add source /src/LanguageIdentification.CLD3/out
RUN dotnet nuget add source /src/LanguageIdentification.FastText.Native/out
RUN dotnet nuget add source /src/LanguageIdentification.FastText/out
RUN dotnet nuget add source /src/LanguageIdentification.Whatlang.Native/out
RUN dotnet nuget add source /src/LanguageIdentification.Whatlang/out

RUN dotnet nuget list source

WORKDIR /src/LanguageIdentification.Tests

ENTRYPOINT ["dotnet", "test", "-c", "Release", "/src/LanguageIdentification.Tests/LanguageIdentification.Tests.csproj"]


