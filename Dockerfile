FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY OrgCheck.csproj ./
RUN dotnet restore OrgCheck.csproj
COPY . .
RUN dotnet publish OrgCheck.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
# "Log" must match LogService's Path.Combine(AppContext.BaseDirectory, "Log", ...) exactly - Linux paths are case-sensitive.
RUN mkdir -p /app/Log /app/data/uploads /app/data/reports
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Render injects PORT at runtime; default it for other container hosts that don't.
ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT} exec dotnet OrgCheck.dll"]
