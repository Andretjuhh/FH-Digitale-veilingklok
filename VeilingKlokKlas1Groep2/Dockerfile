# API build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore
COPY *.csproj ./
RUN dotnet restore ./VeilingKlokKlas1Groep2.csproj

# Build
COPY . .
RUN dotnet publish ./VeilingKlokKlas1Groep2.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "VeilingKlokKlas1Groep2.dll"]
