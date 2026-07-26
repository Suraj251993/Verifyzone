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
# postgresql-client provides psql/pg_restore, used one-time by SetupController to load the DB schema
# from inside Render's network (works around external networks that block TLS on port 5432).
RUN apt-get update && apt-get install -y --no-install-recommends postgresql-client && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
COPY "DB scripts" /app/dbscripts/
COPY orgcheck_17022025.sql /app/dbscripts/

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Render injects PORT at runtime; default it for other container hosts that don't.
ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT} exec dotnet OrgCheck.dll"]
