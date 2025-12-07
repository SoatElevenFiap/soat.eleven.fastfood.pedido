FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY src/*.sln src/
COPY Soat.Eleven.FastFood.Tests/*.csproj Soat.Eleven.FastFood.Tests/
COPY src/Soat.Eleven.FastFood.Api/*.csproj src/Soat.Eleven.FastFood.Api/
COPY src/Soat.Eleven.FastFood.Application/*.csproj src/Soat.Eleven.FastFood.Application/
COPY src/Soat.Eleven.FastFood.Core/*.csproj src/Soat.Eleven.FastFood.Core/
COPY src/Soat.Eleven.FastFood.Infra/*.csproj src/Soat.Eleven.FastFood.Infra/

RUN dotnet restore src/Soat.Eleven.FastFood.sln

COPY . .

RUN dotnet publish "src/Soat.Eleven.FastFood.Api/Soat.Eleven.FastFood.Adapter.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrator

WORKDIR /app

COPY . . 

RUN dotnet tool install --global dotnet-ef --version 8.*

ENV PATH="/root/.dotnet/tools:${PATH}"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

WORKDIR /app

COPY --from=build-env /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Soat.Eleven.FastFood.Adapter.WebApi.dll"]
