FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV TZ=Europe/Prague
ENV LC_ALL=cs_CZ.UTF-8

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "./InvestmentPortfolio/./InvestmentPortfolio.csproj"

# running unit tests
RUN LANG=cs_CZ.UTF-8 IS_RUNNING_AUTOMATED_TEST=true dotnet test InvestmentPortfolio.UnitTests --verbosity quiet

# running integration tests
RUN LANG=cs_CZ.UTF-8 IS_RUNNING_AUTOMATED_TEST=true dotnet test InvestmentPortfolio.IntegrationTests --verbosity quiet

COPY . .
WORKDIR "/src/InvestmentPortfolio"
RUN dotnet build "./InvestmentPortfolio.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InvestmentPortfolio.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InvestmentPortfolio.dll"]