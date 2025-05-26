 
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

 
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./Scienta.Services/*.csproj ./Scienta.Services/  
COPY ./Scienta.Web/*.csproj ./Scienta.Web/  


COPY *.sln .
RUN dotnet restore
COPY . .
RUN dotnet publish ./Scienta.Web/*.csproj -o /publish/
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /src
COPY --from=build /publish .
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT ["dotnet", "Scienta.Web.dll"]

 
 