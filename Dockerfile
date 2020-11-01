#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine
WORKDIR /src
COPY ["DUnit/DUnit.csproj", "DUnit/"]
RUN dotnet restore "DUnit/DUnit.csproj"
COPY . .
WORKDIR "/src/DUnit"
RUN dotnet build "DUnit.csproj" -c Release -o /app/build
RUN dotnet publish "DUnit.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine
WORKDIR /app
COPY --from=0 /app/publish .