FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/GamesReviews.Web/GamesReviews.Web.csproj", "src/GamesReviews.Web/"]
COPY ["src/GamesReviews.Contracts/GamesReviews.Contracts.csproj", "src/GamesReviews.Contracts/"]
COPY ["src/GamesReviews.Domain/GamesReviews.Domain.csproj", "src/GamesReviews.Domain/"]
COPY ["src/GamesReviews.Infrastructure/GamesReviews.Infrastructure.csproj", "src/GamesReviews.Infrastructure/"]
COPY ["src/GamesReviews.Application/GamesReviews.Application.csproj", "src/GamesReviews.Application/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/GamesReviews.Presenters/GamesReviews.Presenters.csproj", "src/GamesReviews.Presenters/"]
RUN dotnet restore "src/GamesReviews.Web/GamesReviews.Web.csproj"
COPY . .
WORKDIR "/src/src/GamesReviews.Web"
RUN dotnet build "GamesReviews.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GamesReviews.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GamesReviews.Web.dll"]
