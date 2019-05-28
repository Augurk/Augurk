FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
ARG Version
ARG InformationalVersion
WORKDIR /app

# copy external library
COPY extLib /extLib

# copy csproj and restore as distinct layers
COPY src/*.sln .
COPY src/**/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore

# copy everything else and build app
COPY src/. ./
WORKDIR /app
RUN dotnet publish /p:Version=$Version /p:InformationalVersion=$InformationalVersion -c Release -o out

# build unit test stage
FROM build AS unit-tests
WORKDIR /app/Augurk.Test
ENTRYPOINT [ "dotnet", "test", "--logger:trx" ]

# build integration test stage
FROM build AS integration-tests
WORKDIR /app/Augurk.IntegrationTest
ENTRYPOINT [ "dotnet", "test", "--logger:trx" ]

# build output image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/Augurk/out ./
ENTRYPOINT ["dotnet", "Augurk.dll"]