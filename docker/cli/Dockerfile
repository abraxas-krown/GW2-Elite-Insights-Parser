# The Elite Insights CLI built from this repository, as an image that only holds its files in /opt/gw2ei.
# It is never run itself: gw2raidar's Dockerfile copies /opt/gw2ei into its app image, which has the .NET
# runtime, and runs GuildWars2EliteInsights-CLI.dll with `dotnet`.
# Published as ghcr.io/abraxas-krown/gw2ei:<tag> by .github/workflows/docker-cli.yml.
# Build from the repository root:  docker build -f docker/cli/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
# Framework-dependent (no runtime bundled) and AnyCPU, so the same files run on x64 and arm64
RUN dotnet publish GW2EI.Executables/GW2EI.Applications/GW2EIParserCLI/GW2EIParserCLI.csproj \
        --configuration Release --output /opt/gw2ei \
    && test -f /opt/gw2ei/GuildWars2EliteInsights-CLI.dll

FROM scratch
COPY --from=build /opt/gw2ei /opt/gw2ei
