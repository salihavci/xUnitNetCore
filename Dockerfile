FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /app
COPY ./xUnitNetCore.Test/*.csproj ./xUnitNetCore.Test/
COPY ./xUnitNetCore.App/*.csproj ./xUnitNetCore.App/
COPY ./xUnitNetCore.Web/*.csproj ./xUnitNetCore.Web/
COPY *.sln .
RUN dotnet restore
COPY . .
RUN dotnet test ./xUnitNetCore.Test/*.csproj
RUN dotnet publish ./xUnitNetCore.Web -o /publish/
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /publish .
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT [ "dotnet" ,"xUnitNetCore.Web.dll" ]