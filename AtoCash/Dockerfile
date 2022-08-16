
FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY AtoCashSol.sln ./
COPY ["AtoCash/AtoCash.csproj", "AtoCash/"]
COPY ["EmailService/EmailService.csproj", "EmailService/"]
RUN dotnet restore "AtoCash/AtoCash.csproj"

COPY . .
WORKDIR "/src/AtoCash"
RUN dotnet build "AtoCash.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AtoCash.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AtoCash.dll"]