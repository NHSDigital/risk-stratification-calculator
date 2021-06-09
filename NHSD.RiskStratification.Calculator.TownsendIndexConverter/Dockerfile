#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

RUN apt-get update && apt-get install -y gnupg

# Add the PostgreSQL PGP key to verify their Debian packages.
# It should be the same key as https://www.postgresql.org/media/keys/ACCC4CF8.asc
RUN apt-key adv --keyserver hkp://p80.pool.sks-keyservers.net:80 --recv-keys B97B0AFCAA1A47F044F244A07FCC7D46ACCC4CF8

# Add PostgreSQL's repository. It contains the most recent stable release
#  of PostgreSQL.
RUN echo "deb http://apt.postgresql.org/pub/repos/apt/ precise-pgdg main" > /etc/apt/sources.list.d/pgdg.list

# Install ``software-properties-common`` and PostgreSQL 11
RUN apt-get update && apt-get install -y software-properties-common postgresql-11

USER postgres

# Create a PostgreSQL role named ``docker`` with ``docker`` as the password and
# then create a database `docker` owned by the ``docker`` role.
# Note: here we use ``&&\`` to run commands one after the other - the ``\``
#       allows the RUN command to span multiple lines.
COPY ["tmp/townsend-data/townsend-data.sql", "/tmp/townsend-data-raw.sql"]
RUN iconv -c -f UTF-16 -t UTF-8 </tmp/townsend-data-raw.sql >/tmp/townsend-data.sql &&\
     /etc/init.d/postgresql start &&\
     psql --command "CREATE USER docker WITH SUPERUSER PASSWORD 'docker';" &&\
     createdb -O docker docker &&\
     psql < /tmp/townsend-data.sql

USER root
RUN rm /tmp/townsend-data.sql

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["NHSD.RiskStratification.Calculator.TownsendIndexConverter/NHSD.RiskStratification.Calculator.TownsendIndexConverter.csproj", "NHSD.RiskStratification.Calculator.TownsendIndexConverter/"]
RUN dotnet restore "NHSD.RiskStratification.Calculator.TownsendIndexConverter/NHSD.RiskStratification.Calculator.TownsendIndexConverter.csproj"
COPY . .
WORKDIR "/src/NHSD.RiskStratification.Calculator.TownsendIndexConverter"
RUN dotnet build "NHSD.RiskStratification.Calculator.TownsendIndexConverter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.RiskStratification.Calculator.TownsendIndexConverter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["NHSD.RiskStratification.Calculator.TownsendIndexConverter/start.sh", "/app/start.sh"]

ENTRYPOINT ["/app/start.sh"]
