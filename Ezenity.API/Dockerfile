# Build
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

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ezenity.API.dll"]
