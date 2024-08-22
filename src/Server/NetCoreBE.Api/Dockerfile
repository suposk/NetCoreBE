#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 6606
EXPOSE 6006

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Server/NetCoreBE.Api/NetCoreBE.Api.csproj", "Server/NetCoreBE.Api/"]
COPY ["Shared/SharedCommon/SharedCommon.csproj", "Shared/SharedCommon/"]
COPY ["Shared/SharedKernel/SharedKernel.csproj", "Shared/SharedKernel/"]
COPY ["Shared/SharedContract/SharedContract.csproj", "Shared/SharedContract/"]
COPY ["Server/NetCoreBE.Application/NetCoreBE.Application.csproj", "Server/NetCoreBE.Application/"]
COPY ["Server/CommonClean/CommonCleanArch.Application/CommonCleanArch.Application.csproj", "Server/CommonClean/CommonCleanArch.Application/"]
COPY ["Server/CommonClean/CommonCleanArch.Domain/CommonCleanArch.Domain.csproj", "Server/CommonClean/CommonCleanArch.Domain/"]
COPY ["Server/NetCoreBE.Domain/NetCoreBE.Domain.csproj", "Server/NetCoreBE.Domain/"]
COPY ["Server/NetCoreBE.Infrastructure/NetCoreBE.Infrastructure.csproj", "Server/NetCoreBE.Infrastructure/"]
COPY ["Server/CommonClean/CommonCleanArch.Infrastructure/CommonCleanArch.Infrastructure.csproj", "Server/CommonClean/CommonCleanArch.Infrastructure/"]
RUN dotnet restore "./Server/NetCoreBE.Api/NetCoreBE.Api.csproj"
COPY . .
WORKDIR "/src/Server/NetCoreBE.Api"
RUN dotnet build "./NetCoreBE.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NetCoreBE.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetCoreBE.Api.dll"]