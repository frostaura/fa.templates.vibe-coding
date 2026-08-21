FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Gaia.Mcp.Server/Gaia.Mcp.Server.csproj src/Gaia.Mcp.Server/
RUN dotnet restore src/Gaia.Mcp.Server/Gaia.Mcp.Server.csproj
COPY src/ src/
RUN dotnet publish src/Gaia.Mcp.Server/Gaia.Mcp.Server.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV GAIA_DATA_DIR=/app/data
# Task/memory/evolution stores are flat JSON — mount this or the data dies with the container.
VOLUME ["/app/data"]
RUN mkdir -p /app/data && chown -R $APP_UID /app/data
USER $APP_UID
EXPOSE 8080
# Probe with a legacy initialize: `ping` was removed in spec 2026-07-28 and the server 400s it (verified).
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -fsS -o /dev/null -X POST http://localhost:8080/mcp -H 'Content-Type: application/json' -H 'Accept: application/json, text/event-stream' \
      -d '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-06-18","capabilities":{},"clientInfo":{"name":"healthcheck","version":"0"}}}' || exit 1
ENTRYPOINT ["dotnet", "Gaia.Mcp.Server.dll"]
