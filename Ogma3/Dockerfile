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
RUN dotnet tool install --global dotnet-ef
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh

FROM build AS publish
RUN dotnet publish "Ogma3.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ogma3.dll"]

#FROM mcr.microsoft.com/dotnet/sdk:5.0
#COPY . /app
#WORKDIR /app
#
#RUN dotnet tool install --global dotnet-ef
#RUN dotnet restore
#RUN dotnet build
#RUN chmod +x ./entrypoint.sh
#
#CMD /bin/bash ./entrypoint.sh