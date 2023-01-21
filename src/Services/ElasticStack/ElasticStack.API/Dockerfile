#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Learned from the following reference project:
# https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Ordering/Ordering.API/Dockerfile
# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "ElasticSearchExample.sln" "ElasticSearchExample.sln"

COPY "Services/ElasticStack/ElasticStack.API/ElasticStack.API.csproj" "Services/ElasticStack/ElasticStack.API/"

COPY "docker-compose.dcproj" "docker-compose.dcproj"

RUN dotnet restore "ElasticSearchExample.sln"

COPY . .
WORKDIR "/src/Services/ElasticStack/ElasticStack.API"
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Template.API.dll"]
