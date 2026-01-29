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
# add any other csproj that API references

RUN dotnet restore ./ProjectEzenity.sln

# Copy everything and publish
COPY . .
RUN dotnet publish Ezenity.API/Ezenity.API.csproj -c Release -o /app/publish

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

# copy full source so EF can find projects, migrations, etc.
COPY . .

# Install EF tool locally (no global install needed)
RUN dotnet tool install --tool-path /tools dotnet-ef --version 8.*
ENV PATH="/tools:${PATH}"

# Default command runs migrations
# Uses env vars from docker compose (.env) at runtime.
CMD ["bash", "-lc", "dotnet ef database update --project Ezenity.Infrastructure --startup-project Ezenity.API"]
