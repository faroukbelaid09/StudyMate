FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["StudyMate.API.csproj", "./"]
RUN dotnet restore "StudyMate.API.csproj"

COPY . .
RUN dotnet publish "StudyMate.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "StudyMate.API.dll"]