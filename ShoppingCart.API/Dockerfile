#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ShoppingCart.API/ShoppingCart.API.csproj", "ShoppingCart.API/"]
COPY ["ShoppingCart.Service/ShoppingCart.Service.csproj", "ShoppingCart.Service/"]
COPY ["ShoppingCart.Data/ShoppingCart.Data.csproj", "ShoppingCart.Data/"]
COPY ["ShoppingCart.Core/ShoppingCart.Core.csproj", "ShoppingCart.Core/"]
RUN dotnet restore "ShoppingCart.API/ShoppingCart.API.csproj"
COPY . .
WORKDIR "/src/ShoppingCart.API"
RUN dotnet build "ShoppingCart.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShoppingCart.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingCart.API.dll"]