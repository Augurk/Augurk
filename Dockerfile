FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
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
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS runtime
WORKDIR /app
COPY --from=build /app/Augurk/out ./
ENTRYPOINT ["dotnet", "Augurk.dll"]