FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Notes/Notes.csproj", "Notes/"]
RUN dotnet restore "Notes/Notes.csproj"
COPY . .
WORKDIR "/src/Notes"
RUN dotnet build "Notes.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notes.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notes.dll"]
