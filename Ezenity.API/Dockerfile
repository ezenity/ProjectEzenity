# =========================
# Restore (SDK) - cached layer
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

# Copy sln + csproj first for restore caching
COPY ProjectEzenity.sln ./
COPY Ezenity.API/Ezenity.API.csproj Ezenity.API/
COPY Ezenity.Core/Ezenity.Core.csproj Ezenity.Core/
COPY Ezenity.Infrastructure/Ezenity.Infrastructure.csproj Ezenity.Infrastructure/
COPY Ezenity.DTOs/Ezenity.DTOs.csproj Ezenity.DTOs/
COPY Ezenity.RazorViews/Ezenity.RazorViews.csproj Ezenity.RazorViews/

# Copy test project csproj(s) ONLY for restore caching (no source yet)
# (This does NOT include tests in runtime, it's only so restore can resolve references)
COPY Ezenity.Tests/Ezenity.Tests.csproj Ezenity.Tests/

# Restore (solution restore is fine since we explicitly copy required csprojs)
RUN dotnet restore ProjectEzenity.sln

# =========================
# Test (SDK)
# =========================
FROM restore AS test
WORKDIR /src

# Now copy the full source and run tests
COPY . .

# If you have multiple test projects, this will run them all:
RUN dotnet test ProjectEzenity.sln -c Release --no-restore --logger "trx;LogFileName=test_results.trx"

# Stricter CI to run:
#RUN dotnet test ProjectEzenity.sln -c Release --no-restore -warnaserror

# =========================
# Build/Publish (SDK)
# =========================
FROM restore AS build
WORKDIR /src
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
##FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrator
FROM restore AS migrator
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
