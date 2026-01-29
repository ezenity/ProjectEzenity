# =========================
# Build (SDK)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy sln + csproj first for restore caching
COPY ProjectEzenity.sln ./
COPY Ezenity.API/Ezenity.API.csproj Ezenity.API/
COPY Ezenity.Core/Ezenity.Core.csproj Ezenity.Core/
COPY Ezenity.Infrastructure/Ezenity.Infrastructure.csproj Ezenity.Infrastructure/
COPY Ezenity.DTOs/Ezenity.DTOs.csproj Ezenity.DTOs/
# If more projects are referenced by the API, COPY them too.

# Restore ONLY the API project (avoids solution projects like Tests/RazorViews)
RUN dotnet restore Ezenity.API/Ezenity.API.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish Ezenity.API/Ezenity.API.csproj -c Release -o /app/publish --no-restore

# =========================
# Runtime (API)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Ezenity.API.dll"]

# =========================
# Migrator (SDK + dotnet-ef)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrator
WORKDIR /src

# Copy full source from the build stage so we have:
# - project files
# - restored packages already in the layer history
# - migrations + startup project
#COPY --from=build /src /src
COPY . .

# Install EF tool locally (no global install needed)
RUN dotnet tool install --tool-path /tools dotnet-ef --version 8.*
ENV PATH="/tools:${PATH}"

# Helpful defaults (safe if overridden by docker compose env)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Default command runs migrations (compose can override if needed)
# Uses env vars from docker compose (.env) at runtime.
CMD ["bash", "-lc", "dotnet ef database update --project Ezenity.Infrastructure --startup-project Ezenity.API"]
