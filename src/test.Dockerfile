FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim

RUN wget https://aka.ms/getvsdbgsh && \
    sh getvsdbgsh -v latest  -l /vsdbg

