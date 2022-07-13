FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FargateDemo/FargateDemo.csproj", "FargateDemo/"]
RUN dotnet restore "FargateDemo/FargateDemo.csproj"
COPY . .
WORKDIR "/src/FargateDemo"
RUN dotnet build "FargateDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FargateDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FargateDemo.dll"]
