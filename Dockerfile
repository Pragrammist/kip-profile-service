# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source


# copy csproj different layers

#this sln file need only for the docker

# Core layer
COPY srcs/Core/*.csproj Core/

# Infrastructure layer
COPY srcs/Infrastructure/*.csproj Infrastructure/

# Web layer
COPY srcs/Web/*.csproj Web/

#sln file to restore all projects
COPY srcs/*.sln .

#restore
RUN dotnet restore

COPY srcs/ .



# copy everything else and build app

RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./



ENTRYPOINT ["dotnet", "Web.dll"]