FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY src/MyAntDesignApp/MyAntDesignApp.csproj src/MyAntDesignApp/
COPY src/MyAntDesignApp.Client/MyAntDesignApp.Client.csproj src/MyAntDesignApp.Client/
RUN dotnet restore src/MyAntDesignApp/MyAntDesignApp.csproj
COPY . .
RUN dotnet publish src/MyAntDesignApp/MyAntDesignApp.csproj -c Release -o /app/publish --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyAntDesignApp.dll"]
