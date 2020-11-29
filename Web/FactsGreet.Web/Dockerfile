#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Web/FactsGreet.Web/FactsGreet.Web.csproj", "Web/FactsGreet.Web/"]
COPY ["Data/FactsGreet.Data/FactsGreet.Data.csproj", "Data/FactsGreet.Data/"]
COPY ["Data/FactsGreet.Data.Models/FactsGreet.Data.Models.csproj", "Data/FactsGreet.Data.Models/"]
COPY ["Data/FactsGreet.Data.Common/FactsGreet.Data.Common.csproj", "Data/FactsGreet.Data.Common/"]
COPY ["Services/FactsGreet.Services.Mapping/FactsGreet.Services.Mapping.csproj", "Services/FactsGreet.Services.Mapping/"]
COPY ["Services/FactsGreet.Services/FactsGreet.Services.csproj", "Services/FactsGreet.Services/"]
COPY ["FactsGreet.Common/FactsGreet.Common.csproj", "FactsGreet.Common/"]
COPY ["Services/FactsGreet.Services.Data/FactsGreet.Services.Data.csproj", "Services/FactsGreet.Services.Data/"]
COPY ["Services/FactsGreet.Services.Messaging/FactsGreet.Services.Messaging.csproj", "Services/FactsGreet.Services.Messaging/"]
COPY ["Web/FactsGreet.Web.Infrastructure/FactsGreet.Web.Infrastructure.csproj", "Web/FactsGreet.Web.Infrastructure/"]
COPY ["Web/FactsGreet.Web.ViewModels/FactsGreet.Web.ViewModels.csproj", "Web/FactsGreet.Web.ViewModels/"]
RUN dotnet restore "Web/FactsGreet.Web/FactsGreet.Web.csproj"
COPY . .
WORKDIR "/src/Web/FactsGreet.Web"
RUN dotnet build "FactsGreet.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FactsGreet.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FactsGreet.Web.dll"]