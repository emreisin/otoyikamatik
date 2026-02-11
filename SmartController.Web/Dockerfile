FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SmartController.Web/SmartController.Web.csproj", "SmartController.Web/"]
COPY ["SmartController.Db/SmartController.Db.csproj", "SmartController.Db/"]
COPY ["SmartController.Shared/SmartController.Shared.csproj", "SmartController.Shared/"]
RUN dotnet restore "SmartController.Web/SmartController.Web.csproj"
COPY . .
RUN dotnet build "SmartController.Web/SmartController.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartController.Web/SmartController.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartController.Web.dll"]
