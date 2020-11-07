#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Ogma3/Ogma3.csproj", "Ogma3/"]
COPY ["Utils/Utils.csproj", "Utils/"]
COPY ["MarkdigExtensions/MarkdigExtensions.csproj", "MarkdigExtensions/"]
RUN dotnet restore "Ogma3/Ogma3.csproj"
COPY . .
WORKDIR "/src/Ogma3"
RUN rm -rf Migrations
RUN dotnet build "Ogma3.csproj" -c Release -o /app/build
#RUN dotnet tool install --global dotnet-ef --version 5.0.0-rc.1.20451.13
#ENV PATH="${PATH}:/root/.dotnet/tools"
#RUN dotnet-ef database update

FROM build AS publish
RUN dotnet publish "Ogma3.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ogma3.dll"]