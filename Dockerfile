# ==============================================================
# Dockerfile general - IglesiaGPS (API + MVC + Consumer + Modelo)
# Corre ambos proyectos en un solo contenedor
# ==============================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000
EXPOSE 10001

# Variable de entorno para que MVC conecte a la API dentro del mismo contenedor
ENV API_BASE_URL=http://localhost:10000

# ---- BUILD ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar TODOS los .csproj primero (cache de restore)
COPY ["IglesiaGPS.modelo/IglesiaGPS.modelo.csproj", "IglesiaGPS.modelo/"]
COPY ["Iglesia.Api.consumer/Iglesia.Api.consumer.csproj", "Iglesia.Api.consumer/"]
COPY ["IglesiaGPS.Api/IglesiaGPS.Api.csproj", "IglesiaGPS.Api/"]
COPY ["Iglesia.MVC/Iglesia.MVC.csproj", "Iglesia.MVC/"]

# Restore de ambos proyectos web (trae todas las dependencias)
RUN dotnet restore "IglesiaGPS.Api/IglesiaGPS.Api.csproj"
RUN dotnet restore "Iglesia.MVC/Iglesia.MVC.csproj"

# Copiar todo el código fuente
COPY . .

# Build API
WORKDIR "/src/IglesiaGPS.Api"
RUN dotnet build "IglesiaGPS.Api.csproj" -c Release -o /app/build/api

# Build MVC
WORKDIR "/src/Iglesia.MVC"
RUN dotnet build "Iglesia.MVC.csproj" -c Release -o /app/build/mvc

# ---- PUBLISH ----
FROM build AS publish

# Publicar API
WORKDIR "/src/IglesiaGPS.Api"
RUN dotnet publish "IglesiaGPS.Api.csproj" -c Release -o /app/publish/api /p:UseAppHost=false

# Publicar MVC
WORKDIR "/src/Iglesia.MVC"
RUN dotnet publish "Iglesia.MVC.csproj" -c Release -o /app/publish/mvc /p:UseAppHost=false

# ---- FINAL ----
FROM base AS final
WORKDIR /app

# Copiar ambas publicaciones
COPY --from=publish /app/publish/api ./api
COPY --from=publish /app/publish/mvc ./mvc

# Script de inicio que lanza API y MVC juntos
RUN echo '#!/bin/bash\n\
echo "Iniciando IglesiaGPS API en puerto 10000..."\n\
cd /app/api && dotnet IglesiaGPS.Api.dll --urls "http://+:10000" &\n\
sleep 3\n\
echo "Iniciando Iglesia MVC en puerto 10001..."\n\
cd /app/mvc && dotnet Iglesia.MVC.dll --urls "http://+:10001" &\n\
wait' > /app/start.sh && chmod +x /app/start.sh

ENTRYPOINT ["/bin/bash", "/app/start.sh"]
