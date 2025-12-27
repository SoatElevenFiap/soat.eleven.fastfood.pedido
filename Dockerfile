FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY src/*.sln src/
COPY Soat.Eleven.Pedidos.Tests/*.csproj Soat.Eleven.Pedidos.Tests/
COPY src/Soat.Eleven.Pedidos.Api/*.csproj src/Soat.Eleven.Pedidos.Api/
COPY src/Soat.Eleven.Pedidos.Application/*.csproj src/Soat.Eleven.Pedidos.Application/
COPY src/Soat.Eleven.Pedidos.Core/*.csproj src/Soat.Eleven.Pedidos.Core/
COPY src/Soat.Eleven.Pedidos.Infra/*.csproj src/Soat.Eleven.Pedidos.Infra/

RUN dotnet restore src/Soat.Eleven.Pedidos.sln

COPY . .

RUN dotnet publish "src/Soat.Eleven.Pedidos.Api/Soat.Eleven.Pedidos.Adapter.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrator

WORKDIR /app

COPY . . 

RUN dotnet tool install --global dotnet-ef --version 8.*

ENV PATH="/root/.dotnet/tools:${PATH}"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

WORKDIR /app

COPY --from=build-env /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Soat.Eleven.Pedidos.Adapter.WebApi.dll"]
