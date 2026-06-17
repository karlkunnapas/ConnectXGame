FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ConnectX.sln ./
COPY Directory.Build.props ./
COPY BLL/BLL.csproj BLL/
COPY DAL/DAL.csproj DAL/
COPY ConsoleApp/ConsoleApp.csproj ConsoleApp/
COPY ConsoleUI/ConsoleUI.csproj ConsoleUI/
COPY MenuSystem/MenuSystem.csproj MenuSystem/
COPY WebApp/WebApp.csproj WebApp/

RUN dotnet restore WebApp/WebApp.csproj

COPY . .
RUN dotnet publish WebApp/WebApp.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "WebApp.dll"]
