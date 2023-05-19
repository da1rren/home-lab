FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM alpine:latest as tailscale
WORKDIR /app
ENV TSFILE=tailscale_1.40.1_amd64.tgz
RUN wget https://pkgs.tailscale.com/stable/${TSFILE} && \
  tar xzf ${TSFILE} --strip-components=1

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Home.Plant.Watering.Shared/Home.Plant.Watering.Shared.csproj", "Home.Plant.Watering.Shared/"]
COPY ["Home.Plant.Watering.Ui/Home.Plant.Watering.Ui.csproj", "Home.Plant.Watering.Ui/"]
RUN dotnet restore "Home.Plant.Watering.Shared/Home.Plant.Watering.Shared.csproj"
RUN dotnet restore "Home.Plant.Watering.Ui/Home.Plant.Watering.Ui.csproj"
COPY . .
WORKDIR "/src/Home.Plant.Watering.Ui"
RUN dotnet build "Home.Plant.Watering.Ui.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Home.Plant.Watering.Ui.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Home.Plant.Watering.Ui/start.sh /app/start.sh
COPY --from=tailscale /app/tailscaled /app/tailscaled
COPY --from=tailscale /app/tailscale /app/tailscale
RUN mkdir -p /var/run/tailscale /var/cache/tailscale /var/lib/tailscale

CMD ["/app/start.sh"]
