#Establece la imagen base del proyecto para utilizar .NET 8 
#y el puerto que utilizará
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

#Establece la imagen que compilará el proyecto, .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release #Configura la compilación como producción
WORKDIR /src
COPY ["App/App.csproj", "App/"]
RUN dotnet restore "./App/App.csproj" #Restaura las dependencias del proyecto
COPY . .
WORKDIR "/src/App"
RUN dotnet build "./App.csproj" -c $BUILD_CONFIGURATION -o /app/build #Compila el proyecto

#Publica el proyecto como producción
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.dll"]