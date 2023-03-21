FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["book-review-api/book-review-api.csproj", "book-review-api/"]
RUN dotnet restore "book-review-api/book-review-api.csproj"
COPY . .
WORKDIR "/src/book-review-api"
RUN dotnet build "book-review-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "book-review-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "book-review-api.dll"]
