FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SOApiProject/SOApiProject.csproj", "SOApiProject/"]
RUN dotnet restore "SOApiProject/SOApiProject.csproj"
COPY . .
WORKDIR "/src/SOApiProject"
RUN dotnet build "SOApiProject.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SOApiProject.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SOApiProject.dll"]