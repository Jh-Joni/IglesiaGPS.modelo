# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["IglesiaGPS.Api/IglesiaGPS.Api.csproj", "IglesiaGPS.Api/"]
COPY ["IglesiaGPS.modelo/IglesiaGPS.modelo.csproj", "IglesiaGPS.modelo/"]
RUN dotnet restore "IglesiaGPS.Api/IglesiaGPS.Api.csproj"

# Copiar el resto de los archivos y compilar
COPY . .
WORKDIR "/src/IglesiaGPS.Api"
RUN dotnet build "IglesiaGPS.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "IglesiaGPS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render expone un puerto dinámico vía la variable de entorno PORT.
# ASP.NET 8+ escucha en el 8080 por defecto, lo cual es compatible con Render.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "IglesiaGPS.Api.dll"]
