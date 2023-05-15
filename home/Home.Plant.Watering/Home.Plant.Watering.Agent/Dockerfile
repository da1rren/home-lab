FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Home.Plant.Watering.Shared/Home.Plant.Watering.Shared.csproj", "Home.Plant.Watering.Shared/"]
COPY ["Home.Plant.Watering.Agent/Home.Plant.Watering.Agent.csproj", "Home.Plant.Watering.Agent/"]
RUN dotnet restore "Home.Plant.Watering.Agent/Home.Plant.Watering.Agent.csproj"
COPY . .
WORKDIR "/src/Home.Plant.Watering.Agent"
RUN dotnet build "Home.Plant.Watering.Agent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Home.Plant.Watering.Agent.csproj" --runtime linux-arm64 --self-contained -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Home.Plant.Watering.Agent.dll"]
