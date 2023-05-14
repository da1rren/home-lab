﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Home.Plant.Watering.Ui/Home.Plant.Watering.Ui.csproj", "Home.Plant.Watering.Ui/"]
RUN dotnet restore "Home.Plant.Watering.Ui/Home.Plant.Watering.Ui.csproj"
COPY . .
WORKDIR "/src/Home.Plant.Watering.Ui"
RUN dotnet build "Home.Plant.Watering.Ui.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Home.Plant.Watering.Ui.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Home.Plant.Watering.Ui.dll"]