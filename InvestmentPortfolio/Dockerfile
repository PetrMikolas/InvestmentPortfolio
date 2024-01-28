FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV TZ=Europe/Prague
ENV LANG=cs_CZ.UTF-8
ENV LANGUAGE=cs_CZ.UTF-8
ENV LC_ALL=cs_CZ.UTF-8

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["InvestmentPortfolio/InvestmentPortfolio.csproj", "InvestmentPortfolio/"]
COPY ["InvestmentPortfolio.Client/InvestmentPortfolio.Client.csproj", "InvestmentPortfolio.Client/"]
RUN dotnet restore "./InvestmentPortfolio/./InvestmentPortfolio.csproj"
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